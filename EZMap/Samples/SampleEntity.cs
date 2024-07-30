using EZMap.Attributes;

namespace EZMap.Samples
{
    public class SampleEntity
    {
        [CrudDefinition(Create = true)]
        public string StringProp1 { get; set; } = string.Empty;
        
        [CrudDefinition(Create = true, Read = true)]
        public int IntProp1 { get; set; }
        
        [CrudDefinition(Update = true)]
        public double DoubleProp1 { get; set; }

        public SampleEntity()
        {
            
        }
    }
}
