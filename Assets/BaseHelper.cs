using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class BaseHelper {

	
	public static int DecToBase(int baseN, int n)
    {
		float num = 0;
		float quotient = n;
		float remainder;
		float i = 0;

		while (quotient != 0)
        {
			remainder = quotient % baseN;
			quotient = Mathf.Floor(quotient / baseN);

			num = (remainder * Mathf.Pow(10, i)) + num;
			i++;
        }

		return (int)num;
    }

	public static int BaseToDec(int baseN, int n)
    {
		int nLen = n.ToString().Length;
		float num = 0;
        for (int i = 0; i < nLen; i++)
        {
			float power = nLen - (i + 1);
			num += Int32.Parse(n.ToString()[i].ToString()) * (Mathf.Pow(baseN, power));
        }

		return (int)num;
    }

	public static int[] FindIndexIn2D(string[,] array, string item)
    {
        for (int row = 0; row < array.GetLength(0); row++)
        {
            for (int col = 0; col < array.GetLength(1); col++)
            {
                if (array[row, col] == item)
                {
                    return new int[] { row, col };
                }
            }
        }
        return null;
    }
}
