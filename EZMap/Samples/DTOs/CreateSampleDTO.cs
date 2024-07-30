namespace EZMap.Samples.DTOs
{
    public class CreateSampleDTO
    {
        public string StringProp1 { get; set; }
        public int IntProp1 { get; set; }

        public CreateSampleDTO(string stringProp1)
        {
            StringProp1 = stringProp1;
        }
        public CreateSampleDTO(string stringProp1, int intProp1)
        {
            StringProp1 = stringProp1;
            IntProp1 = intProp1;
        }
    }
}
