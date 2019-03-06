﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ExRam.Gremlinq.Core;
using ExRam.Gremlinq.Core.GraphElements;
using ExRam.Gremlinq.Core.Tests;
using FluentAssertions;
using Xunit;

namespace ExRam.Gremlinq.Providers.Tests
{
    public abstract class IntegrationTests
    {
        private readonly IConfigurableGremlinQuerySource _g;

        protected IntegrationTests(IConfigurableGremlinQuerySource g)
        {
            _g = g;
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddV()
        {
            var id = Guid.NewGuid().ToString("N");

            var data = await _g
                .AddV(new Language { Id = id, IetfLanguageTag = "en" })
                .ToArrayAsync();

            data.Should().HaveCount(1);
            data[0].Id.Should().Be(id);
            data[0].IetfLanguageTag.Should().Be("en");
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddV_with_DateTimeOffset()
        {
            var now = DateTimeOffset.FromUnixTimeMilliseconds(1481750076295);

            var data = await _g
                .AddV(new Person
                {
                    Name = "Bob",
                    RegistrationDate = now
                })
                .ToArrayAsync();

            data.Should().HaveCount(1);
            data[0].RegistrationDate.Should().Be(now);
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddV_with_DateTime()
        {
            var now = new DateTime(2018, 12, 17, 8, 0, 0, DateTimeKind.Utc);

            var data = await _g
                .AddV(new Company
                {
                    Name = new VertexProperty<string, PropertyValidity>[]
                    {
                        "Company!"
                    },
                    FoundingDate = now
                })
                .ToArrayAsync();

            data.Should().HaveCount(1);
            data[0].FoundingDate.Should().Be(now);
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddV_with_TimeSpan()
        {
            var data = await _g
                .AddV(new TimeFrame
                {
                    StartTime = TimeSpan.FromHours(8),
                    Duration = TimeSpan.FromHours(4)
                })
                .ToArrayAsync();

            data.Should().HaveCount(1);
            data[0].StartTime.Should().Be(TimeSpan.FromHours(8));
            data[0].Duration.Should().Be(TimeSpan.FromHours(4));
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddV_without_id()
        {
            var data = await _g
                .AddV(new Language { IetfLanguageTag = "en" })
                .ToArrayAsync();

            data.Should().HaveCount(1);
            data[0].Id.Should().NotBeNull();
            data[0].IetfLanguageTag.Should().Be("en");
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddV_with_nulls()
        {
            var data = await _g
                .AddV(new Language())
                .ToArrayAsync();

            data.Should().HaveCount(1);
            data[0].Id.Should().NotBeNull();
            data[0].IetfLanguageTag.Should().BeNull();
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddV_with_multi_property()
        {
            var data = await _g
                .AddV(new Company { PhoneNumbers = new[] { "+4912345", "+4923456" } })
                .ToArrayAsync();

            data.Should().HaveCount(1);
            data[0].PhoneNumbers.Should().BeEquivalentTo("+4912345", "+4923456");
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddV_with_Meta_without_properties()
        {
            var data = await _g
                .AddV(new Country { Name = "GER" })
                .ToArrayAsync();

            data.Should().HaveCount(1);
            data[0].Name.Value.Should().Be("GER");
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddV_with_Meta_with_properties()
        {
            var data = await _g
                .AddV(new Country
                {
                    Name = new VertexProperty<string>("GER")
                    {
                        Properties =
                        {
                            { "de", "Deutschland" },
                            { "en", "Germany" }
                        }
                    }
                })
                .ToArrayAsync();

            data.Should().HaveCount(1);
        }
        
        [Fact(Skip = "Integration Test")]
        public async Task AddV_with_enum_property()
        {
            await _g
                .AddV(new Person { Gender = Gender.Female })
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_array_contains_element()
        {
            await _g
                .V<Company>()
                .Where(t => t.PhoneNumbers.Contains("+4912345"))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_array_does_not_contain_element()
        {
            await _g
                .V<Company>()
                .Where(t => !t.PhoneNumbers.Contains("+4912345"))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_array_is_not_empty()
        {
            await _g
                .V<Company>()
                .Where(t => t.PhoneNumbers.Any())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_array_is_empty()
        {
            await _g
                .V<Company>()
                .Where(t => !t.PhoneNumbers.Any())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_array_intersects_aray()
        {
            await _g
                .V<Company>()
                .Where(t => t.PhoneNumbers.Intersect(new[] { "+4912345", "+4923456" }).Any())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_array_does_not_intersect_array()
        {
            await _g
                .V<Company>()
                .Where(t => !t.PhoneNumbers.Intersect(new[] { "+4912345", "+4923456" }).Any())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_array_intersects_empty_array()
        {
            await _g
                .V<Company>()
                .Where(t => t.PhoneNumbers.Intersect(new string[0]).Any())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_array_does_not_intersect_empty_array()
        {
            await _g
                .V<Company>()
                .Where(t => !t.PhoneNumbers.Intersect(new string[0]).Any())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_contained_in_array()
        {
            await _g
                .V<Person>()
                .Where(t => new[] { 36, 37, 38 }.Contains(t.Age))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_contained_in_enumerable()
        {
            var enumerable = new[] { "36", "37", "38" }
                .Select(int.Parse);

            await _g
                .V<Person>()
                .Where(t => enumerable.Contains(t.Age))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_contained_in_empty_enumerable()
        {
            var enumerable = Enumerable.Empty<int>();

            await _g
                .V<Person>()
                .Where(t => enumerable.Contains(t.Age))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_not_contained_in_array()
        {
            await _g
                .V<Person>()
                .Where(t => !new[] { 36, 37, 38 }.Contains(t.Age))
                .ToArrayAsync();
        }
        
        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_not_contained_in_enumerable()
        {
            var enumerable = new[] { "36", "37", "38" }
                .Select(int.Parse);

            await _g
                .V<Person>()
                .Where(t => !enumerable.Contains(t.Age))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_not_contained_in_empty_enumerable()
        {
            var enumerable = Enumerable.Empty<int>();

            await _g
                .V<Person>()
                .Where(t => !enumerable.Contains(t.Age))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_prefix_of_constant()
        {
            await _g
                .V<Country>()
                .Where(c => "+49123".StartsWith(c.CountryCallingCode))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_prefix_of_empty_string()
        {
            await _g
                .V<Country>()
                .Where(c => "".StartsWith(c.CountryCallingCode))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_prefix_of_variable()
        {
            const string str = "+49123";

            await _g
                .V<Country>()
                .Where(c => str.StartsWith(c.CountryCallingCode))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_prefix_of_expression()
        {
            const string str = "+49123xxx";

            await _g
                .V<Country>()
                .Where(c => str.Substring(0, 6).StartsWith(c.CountryCallingCode))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_starts_with_constant()
        {
            await _g
                .V<Country>()
                .Where(c => c.CountryCallingCode.StartsWith("+49"))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_starts_with_empty_string()
        {
            await _g
                .V<Country>()
                .Where(c => c.CountryCallingCode.StartsWith(""))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_disjunction()
        {
            await _g
                .V<Person>()
                .Where(t => t.Age == 36 || t.Age == 42)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_disjunction_with_different_fields()
        {
            await _g
                .V<Person>()
                .Where(t => t.Name.Value == "Some name" || t.Age == 42)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_conjunction()
        {
            await _g
                .V<Person>()
                .Where(t => t.Age == 36 && t.Age == 42)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_complex_logical_expression()
        {
            await _g
                .V<Person>()
                .Where(t => t.Name.Value == "Some name" && (t.Age == 42 || t.Age == 99))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_complex_logical_expression_with_null()
        {
            await _g
                .V<Person>()
                .Where(t => t.Name == null && (t.Age == 42 || t.Age == 99))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_has_conjunction_of_three()
        {
            await _g
                .V<Person>()
                .Where(t => t.Age == 36 && t.Age == 42 && t.Age == 99)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_has_disjunction_of_three()
        {
            await _g
                .V<Person>()
                .Where(t => t.Age == 36 || t.Age == 42 || t.Age == 99)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_conjunction_with_different_fields()
        {
            await _g
                .V<Person>()
                .Where(t => t.Name.Value == "Some name" && t.Age == 42)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_equals_constant()
        {
            await _g
                .V<Person>()
                .Where(t => t.Age == 36)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_equals_expression()
        {
            const int i = 18;

            await _g
                .V<Person>()
                .Where(t => t.Age == i + i)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_equals_converted_expression()
        {
            await _g
                .V<Person>()
                .Where(t => (object)t.Age == (object)36)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_not_equals_constant()
        {
            await _g
                .V<Person>()
                .Where(t => t.Age != 36)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_not_present()
        {
            await _g
                .V<Person>()
                .Where(t => t.Name == null)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_present()
        {
            await _g
                .V<Person>()
                .Where(t => t.Name != null)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_lower_than_constant()
        {
            await _g
                .V<Person>()
                .Where(t => t.Age < 36)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_lower_or_equal_than_constant()
        {
            await _g
                .V<Person>()
                .Where(t => t.Age <= 36)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_bool_property_explicit_comparison1()
        {
            await _g
                .V<TimeFrame>()
                // ReSharper disable once RedundantBoolCompare
                .Where(t => t.Enabled == true)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_bool_property_explicit_comparison2()
        {
            await _g
                .V<TimeFrame>()
                .Where(t => t.Enabled == false)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_bool_property_implicit_comparison1()
        {
            await _g
                .V<TimeFrame>()
                .Where(t => t.Enabled)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_bool_property_implicit_comparison2()
        {
            await _g
                .V<TimeFrame>()
                .Where(t => !t.Enabled)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_greater_than_constant()
        {
            await _g
                .V<Person>()
                .Where(t => t.Age > 36)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_is_greater_or_equal_than_constant()
        {
            await _g
                .V<Person>()
                .Where(t => t.Age >= 36)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_equals_string_constant()
        {
            await _g
                .V<Language>()
                .Where(t => t.Id == (object)"1")
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_equals_local_string_constant()
        {
            const string local = "1";

            await _g
                .V<Language>()
                .Where(t => t.Id == (object)local)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_equals_value_of_anonymous_object()
        {
            var local = new { Value = "1" };

            await _g
                .V<Language>()
                .Where(t => t.Id == (object)local.Value)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_current_element_equals_stepLabel()
        {
            var l = new StepLabel<Language>();

            await _g
                .V<Language>()
                .As(l)
                .V<Language>()
                .Where(l2 => l2 == l)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_equals_stepLabel()
        {
            var l = new StepLabel<string>();

            await _g
                .V<Language>()
                .Values(x => x.IetfLanguageTag)
                .As(l)
                .V<Language>()
                .Where(l2 => l2.IetfLanguageTag == l)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_scalar_element_equals_constant()
        {
            await _g
                .V<Person>()
                .Values(x => x.Age)
                .Where(_ => _ == 36)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_traversal()
        {
            await _g
                .V<Person>()
                .Where(_ => _.Out<LivesIn>())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Where_property_traversal()
        {
            await _g
                .V<Person>()
                .Where(
                    x => x.Age,
                    _ => _
                        .Inject(36))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddE_to_traversal()
        {
            var now = DateTimeOffset.UtcNow;

            await _g
                .AddV(new Person
                {
                    Name = "Bob",
                    RegistrationDate = now
                })
                .AddE(new LivesIn())
                .To(__ => __
                    .V<Country>()
                    .Where(t => t.CountryCallingCode == "+49"))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddE_to_StepLabel()
        {
            await _g
                .AddV(new Language { IetfLanguageTag = "en" })
                .As((_, l) => _
                    .AddV(new Country { CountryCallingCode = "+49" })
                    .AddE<Speaks>()
                    .To(l))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddE_from_traversal()
        {
            var now = DateTimeOffset.UtcNow;

            await _g
                .AddV(new Person
                {
                    Name = "Bob",
                    RegistrationDate = now
                })
                .AddE(new LivesIn())
                .From(__ => __
                    .V<Country>()
                    .Where(t => t.CountryCallingCode == "+49"))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddE_from_StepLabel()
        {
            await _g
                .AddV(new Country { CountryCallingCode = "+49" })
                .As((_, c) => _
                    .AddV(new Language { IetfLanguageTag = "en" })
                    .AddE<Speaks>()
                    .From(c))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddE_InV()
        {
            await _g
                .AddV<Person>()
                .AddE<LivesIn>()
                .To(__ => __
                    .V<Country>("id"))
                .InV()
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task AddE_OutV()
        {
            await _g
                .AddV<Person>()
                .AddE<LivesIn>()
                .To(__ => __
                    .V<Country>("id"))
                .OutV()
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task And()
        {
            await _g
                .V<Person>()
                .And(
                    __ => __
                        .InE<WorksFor>(),
                    __ => __
                        .OutE<LivesIn>())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task And_nested()
        {
            await _g
                .V<Person>()
                .And(
                    __ => __
                        .OutE<LivesIn>(),
                    __ => __
                        .And(
                            ___ => ___
                                .InE<WorksFor>(),
                            ___ => ___
                                .OutE<WorksFor>()))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Or()
        {
            await _g
                .V<Person>()
                .Or(
                    __ => __
                        .InE<WorksFor>(),
                    __ => __
                        .OutE<LivesIn>())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Or_nested()
        {
            await _g
                .V<Person>()
                .Or(
                    __ => __
                        .OutE<LivesIn>(),
                    __ => __
                        .Or(
                            ___ => ___
                                .InE<WorksFor>(),
                            ___ => ___
                                .OutE<WorksFor>()))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Drop()
        {
            await _g
                .V()
                .Drop()
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task FilterWithLambda()
        {
            await _g
                .V<Person>()
                .Where("it.property('str').value().length() == 2")
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Out()
        {
            await _g
                .V<Person>()
                .Out<WorksFor>()
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Out_does_not_include_abstract_edge()
        {
            await _g
                .V<Person>()
                .Out<Edge>()
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task OrderBy_member()
        {
            await _g
                .V<Person>()
                .OrderBy(x => x.Name)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task OrderBy_traversal()
        {
            await _g
                .V<Person>()
                .OrderBy(__ => __.Values(x => x.Name))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task OrderBy_lambda()
        {
            await _g
                .V<Person>()
                .OrderBy("it.property('str').value().length()")
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task OrderByDescending_member()
        {
            await _g
                .V<Person>()
                .OrderByDescending(x => x.Name)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task OrderByDescending_traversal()
        {
            await _g
                .V<Person>()
                .OrderByDescending(__ => __.Values(x => x.Name))
                .ToArrayAsync();
        }
        
        [Fact(Skip = "Integration Test")]
        public async Task OrderBy_ThenBy_member()
        {
            await _g
                .V<Person>()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Age)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task OrderBy_ThenBy_traversal()
        {
            await _g
                .V<Person>()
                .OrderBy(__ => __.Values(x => x.Name))
                .ThenBy(__ => __.Gender)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task OrderBy_ThenBy_lambda()
        {
            await _g
                .V<Person>()
                .OrderBy("it.property('str1').value().length()")
                .ThenBy("it.property('str2').value().length()")
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task OrderBy_ThenByDescending_member()
        {
            await _g
                .V<Person>()
                .OrderBy(x => x.Name)
                .ThenByDescending(x => x.Age)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task OrderBy_ThenByDescending_traversal()
        {
            await _g
                .V<Person>()
                .OrderBy(__ => __.Values(x => x.Name))
                .ThenByDescending(__ => __.Gender)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task SumLocal()
        {
            await _g
                .V<Person>()
                .Values(x => x.Age)
                .SumLocal()
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task SumGlobal()
        {
            await _g
                .V<Person>()
                .Values(x => x.Age)
                .SumGlobal()
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Values_one_member()
        {
            await _g
                .V<Person>()
                .Values(x => x.Name)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Values_two_members()
        {
            await _g
                .V<Person>()
                .Values(x => x.Name, x => x.Id)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Values_three_members()
        {
            await _g
                .V<Person>()
                .Values(x => x.Name, x => x.Gender, x => x.Id)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Values_id_member()
        {
            await _g
                .V<Person>()
                .Values(x => x.Id)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task V_untyped()
        {
            await _g
                .V()
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task OfType_abstract()
        {
            await _g
                .V()
                .OfType<Authority>()
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Repeat_Out()
        {
            await _g
                .V<Person>()
                .Repeat(__ => __
                    .Out<WorksFor>()
                    .OfType<Person>())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Union()
        {
            await _g
                .V<Person>()
                .Union(
                    __ => __.Out<WorksFor>(),
                    __ => __.Out<LivesIn>())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Optional()
        {
            await _g
                .V()
                .Optional(
                    __ => __.Out<WorksFor>())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Not1()
        {
            await _g
                .V()
                .Not(__ => __.Out<WorksFor>())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Not2()
        {
            await _g
                .V()
                .Not(__ => __.OfType<Language>())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Not3()
        {
            await _g
                .V()
                .Not(__ => __.OfType<Authority>())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task As_explicit_label()
        {
            await _g
                .V<Person>()
                .As(new StepLabel<Person>())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Select()
        {
            var stepLabel = new StepLabel<Person>();

            await _g
                .V<Person>()
                .As(stepLabel)
                .Select(stepLabel)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task As_inlined_nested_Select()
        {
            await _g
                .V<Person>()
                .As((_, stepLabel1) => _
                    .As((__, stepLabel2) => __
                        .Select(stepLabel1, stepLabel2)))
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Property_single()
        {
            await _g
                .V<Person>()
                .Property(x => x.Age, 36)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Property_list()
        {
            await _g
                .V<Person>("id")
                .Property(x => x.PhoneNumbers, "+4912345")
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Coalesce()
        {
            await _g
                .V()
                .Coalesce(
                     _ => _
                        .Identity())
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Properties()
        {
            await _g
                .V()
                .Properties()
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Properties_of_member()
        {
            await _g
                .V<Country>()
                .Properties(x => x.Name)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Properties_Where()
        {
            await _g
                .V<Country>()
                .Properties(x => x.Languages)
                .Where(x => x.Value == "de")
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Properties_Where_reversed()
        {
            await _g
                .V<Country>()
                .Properties(x => x.Languages)
                .Where(x => "de" == x.Value)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Meta_Properties()
        {
            await _g
                .V<Country>()
                .Properties(x => x.Name)
                .Properties()
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Meta_Properties_with_key()
        {
            await _g
                .V<Country>()
                .Properties(x => x.Name)
                .Meta<PropertyValidity>()
                .Properties(x => x.ValidFrom)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task Inject()
        {
            await _g
                .Inject(36, 37, 38)
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task WithSubgraphStrategy()
        {
            await _g
                .WithStrategies(new SubgraphQueryStrategy(_ => _.OfType<Person>(), _ => _))
                .V()
                .ToArrayAsync();
        }

        [Fact(Skip = "Integration Test")]
        public async Task WithSubgraphStrategy_empty()
        {
            await _g
                .WithStrategies(new SubgraphQueryStrategy(_ => _, _ => _))
                .V()
                .ToArrayAsync();
        }
    }
}
