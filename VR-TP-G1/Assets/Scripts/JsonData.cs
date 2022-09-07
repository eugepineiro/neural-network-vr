using System;
using System.Collections.Generic;

namespace VRTP3
{
    [Serializable]
    public class JsonData
    {
        public string nn_type;
        public List<int> layers = new List<int>();
        public Kohonen kohonen;
    }
}