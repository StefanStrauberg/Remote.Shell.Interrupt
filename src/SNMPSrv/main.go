package main

import (
	"context"
	"log"
	"os"
	"os/signal"
	"snmpsrv/utils"
	"syscall"

	"github.com/gosnmp/gosnmp"
	"github.com/slayercat/GoSNMPServer"
)

func main() {
	entries, err := utils.ParseSNMPFile("jun2.txt")
	if err != nil {
		log.Fatalf("Error parsing SNMP file: %v", err)
	}

	log.Printf("Loaded %d SNMP entries from file", len(entries))

	// Создаем OID для SNMP сервера из распарсенных данных
	oidItems := utils.CreateOIDItems(entries)

	master := GoSNMPServer.MasterAgent{
		Logger: GoSNMPServer.NewDefaultLogger(),
		SecurityConfig: GoSNMPServer.SecurityConfig{
			AuthoritativeEngineBoots: 1,
			Users: []gosnmp.UsmSecurityParameters{},
		},
		SubAgents: []*GoSNMPServer.SubAgent{
			{
				CommunityIDs: []string{"public"},
				OIDs:         oidItems,
			},
		},
	}

	server := GoSNMPServer.NewSNMPServer(master)
	
	listenAddr := "127.0.0.1:1161"
	
	log.Printf("Starting SNMP server on UDP %s...", listenAddr)
	log.Printf("Community string: 'public'")
	log.Printf("Loaded OIDs: %d", len(oidItems))
	log.Printf("Test with: snmpwalk -v2c -c public %s .1", listenAddr)
	
	err = server.ListenUDP("udp", listenAddr)
	if err != nil {
		log.Fatalf("Error in listen: %v", err)
	}

	// Обработка сигналов для graceful shutdown
	ctx, stop := signal.NotifyContext(context.Background(), os.Interrupt, syscall.SIGTERM)
	defer stop()

	go func() {
		<-ctx.Done()
		log.Println("Shutting down SNMP server...")
		server.Shutdown()
	}()

	log.Println("SNMP server is running. Press Ctrl+C to stop.")

	if err := server.ServeForever(); err != nil {
		log.Printf("ServeForever error: %v", err)
	}
	
	log.Println("SNMP server stopped")
}