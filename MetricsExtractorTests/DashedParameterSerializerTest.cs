using MetricsExtractor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetricsExtractorTests
{
    [TestClass]
    public class DashedParameterSerializerTest
    {
        [TestMethod]
        public void PreenchePropriedadesString()
        {
            var parametros = DashedParameterSerializer.Deserialize<Parametros>(new[] { "-nome", "Alberto", "-email", "alberto.monteiro@live.com" });

            Assert.AreEqual("Alberto", parametros.Nome);
            Assert.AreEqual("alberto.monteiro@live.com", parametros.Email);
        }

        [TestMethod]
        public void PreenchePropriedadesArray()
        {
            var parametros = DashedParameterSerializer.Deserialize<Parametros>(new[] { "-nome", "Alberto", "-telefones", "302110836;8234779", "-email", "alberto.monteiro@live.com" });

            Assert.AreEqual("Alberto", parametros.Nome);
            Assert.AreEqual("alberto.monteiro@live.com", parametros.Email);
            CollectionAssert.AreEqual(new[] { "302110836", "8234779" }, parametros.Telefones);
        }

        [TestMethod]
        public void IgnoraPropriedadesInexistentes()
        {
            var parametros = DashedParameterSerializer.Deserialize<Parametros>(new[] { "-nome", "Alberto", "-naoExiste", "naoExiste", "-email", "alberto.monteiro@live.com" });

            Assert.AreEqual("Alberto", parametros.Nome);
            Assert.AreEqual("alberto.monteiro@live.com", parametros.Email);
        }

        class Parametros
        {
            public string Nome { get; set; }

            public string Email { get; set; }
            public string[] Telefones { get; set; }
        }
    }
}
