using System;

namespace EZ_B {

  public class ValuePair {

    public object Key = string.Empty;
    public string Text = string.Empty;

    public int KeyAsInt {
      get {
        return Convert.ToInt32(Key);
      }
    }

    public string KeyAsString {
      get {
        return Key.ToString();
      }
    }

    public double KeyAsDouble {
      get {
        return Convert.ToDouble(Key);
      }
    }

    public ValuePair() {
    }

    public ValuePair(object key, string text) {

      Key = key;
      Text = text;
    }

    public ValuePair(string textAndKey) {

      Key = textAndKey;
      Text = textAndKey;
    }

    public ValuePair(string key, string text) {

      Key = key;
      Text = text;
    }

    public ValuePair(int key, string text) {

      Key = key;
      Text = text;
    }

    public ValuePair(double key, string text) {

      Key = key;
      Text = text;
    }

    public override string ToString() {

      return Text;
    }
  }
}
