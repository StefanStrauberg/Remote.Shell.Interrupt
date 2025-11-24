package utils

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
	"strings"

	"github.com/gosnmp/gosnmp"
	"github.com/slayercat/GoSNMPServer"
)

// SNMPEntry представляет распарсенную запись из файла
type SNMPEntry struct {
	OID      string
	Type     string
	Value    string
	RawValue string
}

// ParseSNMPFile парсит файл с SNMP выводами, включая многострочные записи
func ParseSNMPFile(filename string) ([]SNMPEntry, error) {
	file, err := os.Open(filename)
	if err != nil {
		return nil, err
	}
	defer file.Close()

	var entries []SNMPEntry
	scanner := bufio.NewScanner(file)

	var currentEntry *SNMPEntry
	var continuationLines []string

	for scanner.Scan() {
		line := strings.TrimSpace(scanner.Text())
		if line == "" {
			continue
		}

		// Проверяем, начинается ли строка с OID (новая запись)
		if isOIDLine(line) {
			// Сохраняем предыдущую запись если она есть
			if currentEntry != nil {
				// Объединяем все continuation lines
				if len(continuationLines) > 0 {
					currentEntry.RawValue += " " + strings.Join(continuationLines, " ")
					currentEntry.Value += " " + strings.Join(continuationLines, " ")
				}
				entries = append(entries, *currentEntry)
				continuationLines = []string{}
			}

			// Парсим новую запись
			entry, err := parseSNMPLine(line)
			if err != nil {
				log.Printf("Warning: skipping invalid line '%s': %v", line, err)
				continue
			}
			currentEntry = &entry
		} else if currentEntry != nil {
			// Это продолжение предыдущей записи
			continuationLines = append(continuationLines, line)
		}
	}

	// Не забываем добавить последнюю запись
	if currentEntry != nil {
		if len(continuationLines) > 0 {
			currentEntry.RawValue += " " + strings.Join(continuationLines, " ")
			currentEntry.Value += " " + strings.Join(continuationLines, " ")
		}
		entries = append(entries, *currentEntry)
	}

	if err := scanner.Err(); err != nil {
		return nil, err
	}

	return entries, nil
}

// isOIDLine проверяет, начинается ли строка с OID
func isOIDLine(line string) bool {
	// Проверяем основные префиксы OID
	prefixes := []string{"iso.", "1.3.6.1.", ".1.3.6.1."}
	for _, prefix := range prefixes {
		if strings.HasPrefix(line, prefix) {
			return true
		}
	}
	return false
}

// parseSNMPLine парсит одну строку SNMP вывода
func parseSNMPLine(line string) (SNMPEntry, error) {
	// Разделяем строку на OID и значение
	parts := strings.SplitN(line, "=", 2)
	if len(parts) != 2 {
		return SNMPEntry{}, fmt.Errorf("invalid format, expected '='")
	}

	oid := strings.TrimSpace(parts[0])
	valuePart := strings.TrimSpace(parts[1])

	// Парсим тип и значение
	var dataType, value string
	
	// Разделяем тип и значение
	typeValueParts := strings.SplitN(valuePart, ":", 2)
	if len(typeValueParts) == 2 {
		dataType = strings.TrimSpace(typeValueParts[0])
		value = strings.TrimSpace(typeValueParts[1])
	} else {
		// Если нет явного типа, пробуем определить по формату
		dataType = "STRING"
		value = valuePart
	}

	// Конвертируем OID из точечной нотации в числовую
	numericOID := convertOIDToNumeric(oid)

	return SNMPEntry{
		OID:      numericOID,
		Type:     dataType,
		Value:    value,
		RawValue: valuePart,
	}, nil
}

// convertOIDToNumeric конвертирует OID из строкового формата в числовой
func convertOIDToNumeric(oid string) string {
	// Удаляем префиксы iso, internet и т.д.
	oid = strings.TrimPrefix(oid, "iso.")
	oid = strings.TrimPrefix(oid, "internet.")
	
	// Заменяем текстовые префиксы на числовые
	oid = strings.ReplaceAll(oid, "iso", "1")
	oid = strings.ReplaceAll(oid, "org", "3")
	oid = strings.ReplaceAll(oid, "dod", "6")
	oid = strings.ReplaceAll(oid, "internet", "1")
	oid = strings.ReplaceAll(oid, "mgmt", "2")
	oid = strings.ReplaceAll(oid, "mib-2", "1")
	oid = strings.ReplaceAll(oid, "private", "4")
	oid = strings.ReplaceAll(oid, "enterprises", "1")
	
	return oid
}

// CreateOIDItems создает PDUValueControlItem из распарсенных записей
func CreateOIDItems(entries []SNMPEntry) []*GoSNMPServer.PDUValueControlItem {
	var oidItems []*GoSNMPServer.PDUValueControlItem

	for _, entry := range entries {
		item := &GoSNMPServer.PDUValueControlItem{
			OID:  entry.OID,
			Type: getSNMPType(entry.Type),
			OnGet: func(entry SNMPEntry) func() (interface{}, error) {
				return func() (interface{}, error) {
					return convertValue(entry.Value, entry.Type)
				}
			}(entry),
			Document: fmt.Sprintf("From file: %s", entry.RawValue),
		}
		oidItems = append(oidItems, item)
	}

	return oidItems
}

// getSNMPType конвертирует строковый тип в gosnmp тип
func getSNMPType(typeStr string) gosnmp.Asn1BER {
	switch strings.ToUpper(typeStr) {
	case "STRING", "OCTETSTRING":
		return gosnmp.OctetString
	case "INTEGER", "INT":
		return gosnmp.Integer
	case "COUNTER32", "COUNTER":
		return gosnmp.Counter32
	case "GAUGE32", "GAUGE":
		return gosnmp.Gauge32
	case "TIMETICKS":
		return gosnmp.TimeTicks
	case "IPADDRESS":
		return gosnmp.IPAddress
	case "OID":
		return gosnmp.ObjectIdentifier
	case "HEX-STRING":
		return gosnmp.OctetString
	default:
		// По умолчанию используем OctetString
		return gosnmp.OctetString
	}
}

// convertValue конвертирует строковое значение в соответствующий тип
func convertValue(valueStr, typeStr string) (interface{}, error) {
	valueStr = strings.Trim(valueStr, `" `)
	
	switch strings.ToUpper(typeStr) {
	case "STRING", "OCTETSTRING":
		return valueStr, nil
		
	case "INTEGER", "INT":
		val, err := strconv.Atoi(valueStr)
		if err != nil {
			// Если не удалось преобразовать, возвращаем как строку
			return valueStr, nil
		}
		return val, nil
		
	case "COUNTER32", "COUNTER":
		val, err := strconv.ParseUint(valueStr, 10, 32)
		if err != nil {
			return uint32(0), nil
		}
		return uint32(val), nil
		
	case "GAUGE32", "GAUGE":
    val, err := strconv.ParseUint(valueStr, 10, 64)
    if err != nil {
        log.Printf("Warning: invalid Gauge32 value %s, defaulting to 0", valueStr)
        return uint32(0), nil
    }
    if val > 4294967295 {
        log.Printf("Warning: Gauge32 value %s too large, using max uint32", valueStr)
        return uint32(4294967295), nil
    }
    return uint32(val), nil
		
	case "TIMETICKS":
		// Убираем скобки если есть
		valueStr = strings.Trim(valueStr, "()")
		// Убираем текстовое описание дней/времени
		if idx := strings.Index(valueStr, ")"); idx != -1 {
			valueStr = valueStr[:idx]
			valueStr = strings.Trim(valueStr, "()")
		}
		val, err := strconv.ParseUint(valueStr, 10, 32)
		if err != nil {
			return uint32(0), nil
		}
		return uint32(val), nil
		
	case "IPADDRESS":
		return valueStr, nil
		
	case "OID":
		converted, err := convertAndValidateOID(valueStr)
		if err != nil {
			return nil, err
		}
		return converted, nil
		
	case "HEX-STRING":
		// Убираем пробелы из HEX строки и объединяем все строки
		hexStr := strings.ReplaceAll(valueStr, " ", "")
		// Конвертируем HEX строку в байты
		var bytes []byte
		for i := 0; i < len(hexStr); i += 2 {
			if i+1 >= len(hexStr) {
				break
			}
			b, err := strconv.ParseUint(hexStr[i:i+2], 16, 8)
			if err != nil {
				// Если ошибка, возвращаем как есть
				return valueStr, nil
			}
			bytes = append(bytes, byte(b))
		}
		return string(bytes), nil
		
	default:
		return valueStr, nil
	}
}

// convertAndValidateOID конвертирует и валидирует OID
func convertAndValidateOID(oid string) (string, error) {
    numericOID := convertOIDToNumeric(oid)
    parts := strings.Split(numericOID, ".")
    var validatedParts []string

    for _, part := range parts {
        if part == "" {
            continue
        }
        num, err := strconv.ParseUint(part, 10, 64)
        if err != nil {
            return "", fmt.Errorf("invalid OID component '%s': %v", part, err)
        }

        // GoSNMPServer внутри использует ParseInt(..., 32), значит максимум int32
        if num > 2147483647 {
            log.Printf("Warning: OID component %d too large, truncating to %d", num, 2147483647)
            num = 2147483647
        }

        validatedParts = append(validatedParts, strconv.FormatUint(num, 10))
    }

    return strings.Join(validatedParts, "."), nil
}

