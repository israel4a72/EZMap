namespace EZMap.Attributes
{
    internal class CrudDefinition : Attribute
    {
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
    }
}
