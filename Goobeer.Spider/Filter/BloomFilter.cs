using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Goobeer.Spider
{
    [Serializable]
    public class BloomFilter
    {
        private BitArray hashbits;
        private int numKeys;
        private int[] hashKeys;

        public BloomFilter(int tableSize, int nKeys)
        {
            numKeys = nKeys;
            hashKeys = new int[numKeys];
            hashbits = new BitArray(tableSize);
        }

        private int HashString(string s)
        {
            int hash = 0;

            for (int i = 0; i < s.Length; i++)
            {
                hash += s[i];
                hash += (hash << 3);
                hash ^= (hash >> 5);
            }
            hash += (hash << 7);
            hash ^= (hash >> 11);
            hash += (hash << 9);
            return hash;
        }

        private void CreateHashes(string str)
        {
            int hash1 = str.GetHashCode();
            int hash2 = HashString(str);

            hashKeys[0] = Math.Abs(hash1 % hashbits.Count);
            if (numKeys > 1)
            {
                for (int i = 1; i < numKeys; i++)
                {
                    hashKeys[i] = Math.Abs((hash1 + (i * hash2))
                        % hashbits.Count);
                }
            }
        }

        public bool Test(string str)
        {
            CreateHashes(str);
            // Test each hash key.  Return false if any 
            //  one of the bits is not set.
            foreach (int hash in hashKeys)
            {
                if (!hashbits[hash])
                    return false;
            }
            // All bits set.  The item is there.
            return true;
        }

        /// <summary>
        /// 是否包含 str 字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool Add(string str)
        {
            // Initially assume that the item is in the table
            bool rslt = true;
            CreateHashes(str);
            foreach (int hash in hashKeys)
            {
                if (!hashbits[hash])
                {
                    // One of the bits wasn't set, so show that
                    // the item wasn't in the table, and set that bit.
                    rslt = false;
                    hashbits[hash] = true;
                }
            }
            return rslt;
        }
    }
}
