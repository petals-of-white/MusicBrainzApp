using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MusicBrainz.Common.Entities;
using Xunit;
using Xunit.Abstractions;

namespace MusicBrainz.Tests
{
    public class ModelsTest
    {
        private readonly ITestOutputHelper output;

        public ModelsTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        public static IEnumerable<object []> GetModelWithWrongData =>
        new List<object []>
        {
            new object[]{ new Area {Name="Hoho"} },
            new object[]{ new Recording { Length=-1, Comment="An interesting one" } },
            new object[]{ new Artist {Name="Very Good", Comment="Marvellous" } },
            new object[]{ new Release {Name="SuperRelase",} },
            new object[]{ new ReleaseGroup {Name="Groupy Group", EditsPending=-10 } },
            new object[]{ new Work {} },
            new object[]{ new Place { Id=128, Comment="Marvellous" } },
            new object[]{ new Label {Comment="Goodo", SortName="Interesting"} },
            new object[]{ new Url  {LastUpdated=DateTime.Now,  } },
        };

        [Theory]
        [MemberData(nameof(GetModelWithWrongData))]
        public void EmptyModelValidation_ShouldFail(object model)
        {
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results);

            foreach (ValidationResult result in results)
            {
                output.WriteLine(result.ErrorMessage);
            }
            Assert.False(isValid);
        }
    }
}