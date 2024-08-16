using System.Text;

namespace Remote.Shell.Interrupt.Storehouse.Domain.BusinessLogic;

public class BusinessRule : BaseEntity
{
  public string Name { get; set; } = string.Empty; // Get Interfaces for Huawei
  public Expression<Func<object, bool>>? Condition { get; set; } // x.Name == Huawei
  public uint BranchNumber
  {
    get
    {
      return ConvertStringToUInt(StringBranchNumber);
    }
    set
    {
      Append(value);
    }
  } // 1315

  public string StringBranchNumber { get; private set; } = string.Empty; // 1.3.15
  public int SequenceNumber { get; set; } // 15
  public Assignment? Assignment { get; set; } // Represent what to do

  static uint ConvertStringToUInt(string input)
  {
    // remove dots
    string cleanedString = input.Replace(".", "");

    // converting to digit
    if (uint.TryParse(cleanedString, out uint result))
      return result;
    else
      throw new FormatException("The cleaned string is not a valid ulong number.");
  }

  private void Append(uint value)
  {
    if (String.IsNullOrEmpty(StringBranchNumber))
    {
      StringBranchNumber = value.ToString();
      BranchNumber = value;
    }
    else
    {

    }
  }
}