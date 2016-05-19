namespace EZ_B.Classes {

  public class BV4615Response {

    public bool IsValid = false;
    public byte Value = 0;
    public bool Toggle = false;
    public byte Byte1 = 0;

    public string Byte1BinaryStr {
      get {
           return Functions.ByteToBinaryString(Byte1);
      }
    }

    public string Byte2BinaryStr {
      get {
        return Functions.ByteToBinaryString(Value);
      }
    }
      
    public BV4615Response(bool toggle, bool isValid, byte byte1, byte byte2) {

      IsValid = isValid;
      Value = byte2;
      Toggle = toggle;
      Byte1 = byte1;
    }

    public override string ToString() {

      return string.Format("Toggle: {0}, Valid: {1}, Value: {2}", Toggle, IsValid, Value);
    }
  }
}
