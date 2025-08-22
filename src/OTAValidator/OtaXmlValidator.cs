using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace OTAValidator
{
    public class OtaXmlValidator
    {
        private readonly string _schemaPath;

        public OtaXmlValidator(string schemaPath)
        {
            if (string.IsNullOrWhiteSpace(schemaPath))
                throw new ArgumentException("O caminho do schema não pode ser vazio.", nameof(schemaPath));

            if (!File.Exists(schemaPath))
                throw new FileNotFoundException("Arquivo XSD não encontrado.", schemaPath);

            _schemaPath = schemaPath;
        }

        public bool Validate(string xmlPath)
        {
            if (string.IsNullOrWhiteSpace(xmlPath))
                throw new ArgumentException("O caminho do XML não pode ser vazio.", nameof(xmlPath));

            if (!File.Exists(xmlPath))
                throw new FileNotFoundException("Arquivo XML não encontrado.", xmlPath);

            bool isValid = true;
            var settings = new XmlReaderSettings();

            settings.Schemas.Add(null, _schemaPath);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += (sender, args) =>
            {
                isValid = false;
                Console.WriteLine($"Erro de validação: {args.Message}");
            };

            using var reader = XmlReader.Create(xmlPath, settings);
            try
            {
                while (reader.Read()) { } // Percorre todo o XML
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"Erro de XML: {ex.Message}");
                return false;
            }

            return isValid;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string schemaFile = "ota-schema.xsd";   // Caminho para seu XSD OTA
            string xmlFile = "sample-ota-request.xml"; // Caminho para um XML OTA válido

            try
            {
                var validator = new OtaXmlValidator(schemaFile);
                bool result = validator.Validate(xmlFile);

                Console.WriteLine(result
                    ? "✅ XML válido conforme o schema OTA."
                    : "❌ XML inválido. Verifique os erros listados acima.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro durante a validação: {ex.Message}");
            }
        }
    }
}
