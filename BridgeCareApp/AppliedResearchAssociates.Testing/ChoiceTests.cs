using Newtonsoft.Json;
using Xunit;

namespace AppliedResearchAssociates.Testing
{
    public class ChoiceTests
    {
        [Fact]
        public void NewtonsoftJsonSerializationRoundtripAsMember()
        {
            var subject = new ChoiceSerializationTestSubject { Member = 123 };
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            var json = JsonConvert.SerializeObject(subject, settings);
            var subjectAgain = JsonConvert.DeserializeObject<ChoiceSerializationTestSubject>(json, settings);
            var comparer = SelectionEqualityComparer<ChoiceSerializationTestSubject>.Create(_ => _.Member);
            var same = comparer.Equals(subjectAgain, subject);
            Assert.True(same);
        }

        [Fact]
        public void NewtonsoftJsonSerializationRoundtripAsRoot()
        {
            var subject = Choice<int, string>.Of(123);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            var json = JsonConvert.SerializeObject(subject, typeof(Choice<int, string>), settings);
            var subjectAgain = JsonConvert.DeserializeObject<Choice<int, string>>(json, settings);
            Assert.Equal(subject, subjectAgain);
        }
    }
}
