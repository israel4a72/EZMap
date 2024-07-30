using EZMap.Exceptions;
using EZMap.Samples;
using EZMap.Samples.DTOs;

namespace EZMap.Test
{
    [TestClass]
    public class MappingTest
    {
        [TestMethod]
        public void Should_Result_NonNull_CreateDTO()
        {
            var origin = new CreateSampleDTO("3")
            {
                IntProp1 = 2
            };

            SampleEntity? result = Mapper<CreateSampleDTO>.MapToCreate<SampleEntity>(origin);

            Assert.IsNotNull(result, "Retornou objeto anulado.");
        }
        [TestMethod]
        public void Should_Result_NonNull_CreateDTO_Array()
        {
            var origin = new CreateSampleDTO[]{
                new("3")
                {
                    IntProp1 = 2
                },
                new ("6")
                {
                    IntProp1 = 2
                }
            };

            SampleEntity[] result = Mapper<CreateSampleDTO>.MapToCreate<SampleEntity>(origin).ToArray();
            Assert.AreEqual(origin.Length, result.Length, "Quantidade de itens do objeto de origem para o de destino é diferente.");
        }
        [TestMethod]
        public void Should_Result_NonNull_ReadDTO()
        {
            var origin = new SampleEntity()
            {
                DoubleProp1 = 1,
                IntProp1 = 2,
                StringProp1 = "3"
            };

            ReadSampleDTO? result = Mapper<SampleEntity>.MapToRead<ReadSampleDTO>(origin);

            Assert.IsNotNull(result, "Retornou objeto anulado.");
        }
        [TestMethod]
        public void Should_Result_NonNull_ReadDTO_Array()
        {
            var origin = new SampleEntity[]
            {
                new() {
                    DoubleProp1 = 1,
                    IntProp1 = 2,
                    StringProp1 = "3"
                },
                new() {
                    DoubleProp1 = 4,
                    IntProp1 = 5,
                    StringProp1 = "6"
                }
            };

            ReadSampleDTO[] result = Mapper<SampleEntity>.MapToRead<ReadSampleDTO>(origin).ToArray();
            Assert.AreEqual(origin.Length, result.Length, "Quantidade de itens do objeto de origem para o de destino é diferente.");
        }
    }
}