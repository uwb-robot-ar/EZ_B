using System;
using System.Collections.Generic;

namespace EZ_B {

  public class SoundFilters {

    public static byte[] TimeStretch(byte[] buffer, int loopCount, int sampleSize) {

      List<byte> _buffer = new List<byte>();
      _buffer.AddRange(buffer);

      List<byte> newBuff = new List<byte>();

      for (int pos=0; pos < _buffer.Count; pos += sampleSize) {

        int endPos = Math.Min(sampleSize, _buffer.Count - pos);

        for (int rep=0; rep < loopCount; rep++)
          newBuff.AddRange(_buffer.GetRange(pos, endPos));
      }

      return newBuff.ToArray();
    }
  }
}
