using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Редактор_тестов_на_XML
{
    [Serializable]
    public class Test
    {
        [XmlAttribute]
        public string TestId { get; set; }
        [XmlAttribute]
        public string TestType { get; set; }
        public string Name { get; set; }
        public string CommandLine { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public Boolean CanBeSkipped { get; set; }

        public Test()
        { }
        public Test(string testId, string testType, string name, string commandLine, string input, string output, Boolean canBeSkipped)
        {
            TestId = testId;
            TestType = testType;
            Name = name;
            CommandLine = commandLine;
            Input = input;
            Output = output;
            CanBeSkipped = canBeSkipped;
        }
    }

}
