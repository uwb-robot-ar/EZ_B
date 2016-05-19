using System;

namespace EZ_B {

  public class RandomUnique {

    private int    _lastRandom = -1;
    private Random _random     = new Random();

    /// <summary>
    /// Create an instance of the RandomUnique Class which attempts to provide a unique random number and other random functions
    /// </summary>
    public RandomUnique() {

    }

    /// <summary>
    /// Return a random number within specified range.
    /// Using this random number generating function will provide a common seed.
    /// </summary>
    public int GetRandomNumber(int lowestVal, int highestVal) {

      return _random.Next(lowestVal, highestVal);
    }

    /// <summary>
    /// Return a random number and tries to make the returned value unique from the last time this function was called.
    /// </summary>
    public int GetRandomUniqueNumber(int lowestVal, int highestVal) {

      if (lowestVal == highestVal)
        return lowestVal;

      for (int x=0; x < 10; x++) {

        int tmp = GetRandomNumber(lowestVal, highestVal);

        if (tmp != _lastRandom) {

          _lastRandom = tmp;

          return tmp;
        }
      }

      return GetRandomNumber(lowestVal, highestVal);
    }
  }
}
