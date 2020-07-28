using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Model
{
    public class BaseModel
    {
        public int id { get; set; }

        [JsonIgnore]
        public IDictionary<Type, object> dictionaryEntities = new Dictionary<Type, object>();
        
        public virtual void MappingModelToEntities()
        {
            dictionaryEntities.Clear();
        }
    }
}
