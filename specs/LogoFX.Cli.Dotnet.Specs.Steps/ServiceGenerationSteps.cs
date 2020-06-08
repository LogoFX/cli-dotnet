using System.IO;
using TechTalk.SpecFlow;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    [Binding]
    internal sealed class ServiceGenerationSteps
    {
        [Then(@"The folder '(.*)' contains generated service entity objects for name '(.*)' for solution name '(.*)'")]
        public void ThenTheFolderContainsGeneratedServiceEntityObjectsForNameForSolutionName(string folderName,
            string entityName,
            string solutionName)
        {
            var tempPath = Path.GetTempPath();

            var structure = new GeneratedFolder(tempPath, folderName)
                .WithFolder($"{solutionName}.Data.Contracts.Providers",
                    r => r.WithFile($"I{entityName}DataProvider.cs",
                        $@"using System;
using System.Collections.Generic;
using {solutionName}.Data.Contracts.Dto;

namespace {solutionName}.Data.Contracts.Providers
{{
    public interface I{entityName}DataProvider
    {{
        IEnumerable<{entityName}Dto> GetItems();

        bool DeleteItem(Guid id);

        bool UpdateItem({entityName}Dto dto);

        void CreateItem({entityName}Dto dto);
    }}
}}")).WithFolder($"{solutionName}.Model.Contracts",
                    r => r.WithFile($"I{entityName}Service.cs",
                        $@"using System.Collections.Generic;
using System.Threading.Tasks;

namespace {solutionName}.Model.Contracts
{{
    public interface I{entityName}Service
    {{
        IEnumerable<I{entityName}> Items {{ get; }}

        Task GetItems();

        Task<I{entityName}> NewItem();

        Task SaveItem(I{entityName} item);

        Task DeleteItem(I{entityName} item);
    }}
}}")).WithFolder($"{solutionName}.Data.Fake.Containers",
                    r => r.WithFile($"{entityName}DataContainer.cs",
                        $@"using System.Collections.Generic;
using {solutionName}.Data.Contracts.Dto;
using {solutionName}.Data.Fake.Containers.Contracts;

namespace {solutionName}.Data.Fake.Containers
{{
    public interface I{entityName}DataContainer : IDataContainer
    {{
        IEnumerable<{entityName}Dto> Items {{ get; }}
    }}

    public sealed class {entityName}DataContainer : I{entityName}DataContainer
    {{
        private readonly List<{entityName}Dto> _items = new List<{entityName}Dto>();
        public IEnumerable<{entityName}Dto> Items => _items;

        public void UpdateItems(IEnumerable<{entityName}Dto> items)
        {{
            _items.Clear();
            _items.AddRange(items);
        }}
    }}
}}")).WithFolder($"{solutionName}.Data.Fake.ProviderBuilders",
                    r => r.WithFile($"{entityName}ProviderBuilder.cs",
                        $@"using System;
using System.Collections.Generic;
using System.Linq;
using Attest.Fake.Builders;
using Attest.Fake.Core;
using Attest.Fake.Setup.Contracts;
using {solutionName}.Data.Contracts.Dto;
using {solutionName}.Data.Contracts.Providers;

namespace {solutionName}.Data.Fake.ProviderBuilders
{{
    public sealed class {entityName}ProviderBuilder : FakeBuilderBase<I{entityName}DataProvider>.WithInitialSetup
    {{
        private readonly List<{entityName}Dto> _itemsStorage = new List<{entityName}Dto>();

        private {entityName}ProviderBuilder()
        {{

        }}

        public static {entityName}ProviderBuilder CreateBuilder() => new {entityName}ProviderBuilder();

        public void WithItems(IEnumerable<{entityName}Dto> items)
        {{
            _itemsStorage.Clear();
            _itemsStorage.AddRange(items);
        }}

        protected override IServiceCall<I{entityName}DataProvider> CreateServiceCall(
            IHaveNoMethods<I{entityName}DataProvider> serviceCallTemplate) => serviceCallTemplate
            .AddMethodCallWithResult(t => t.GetItems(),
                r => r.Complete(GetItems))
            .AddMethodCallWithResult<Guid, bool>(t => t.DeleteItem(It.IsAny<Guid>()),
                (r, id) => r.Complete(DeleteItem(id)))
            .AddMethodCallWithResult<{entityName}Dto, bool>(t => t.UpdateItem(It.IsAny<{entityName}Dto>()),
                (r, dto) => r.Complete(k =>
                {{
                    SaveItem(k);
                    return true;
                }}))
            .AddMethodCall<{entityName}Dto>(t => t.CreateItem(It.IsAny<{entityName}Dto>()),
                (r, dto) => r.Complete(SaveItem));

        private IEnumerable<{entityName}Dto> GetItems() => _itemsStorage;

        private bool DeleteItem(Guid id)
        {{
            var dto = _itemsStorage.SingleOrDefault(x => x.Id == id);
            return dto != null && _itemsStorage.Remove(dto);
        }}

        private void SaveItem({entityName}Dto dto)
        {{
            var oldDto = _itemsStorage.SingleOrDefault(x => x.Id == dto.Id);
            if (oldDto == null)
            {{
                _itemsStorage.Add(dto);
            }}
        }}
    }}
}}")).WithFolder($"{solutionName}.Data.Fake.Providers",
                    r => r.WithFile($"Fake{entityName}DataProvider.cs",
                        $@"using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attest.Fake.Builders;
using JetBrains.Annotations;
using {solutionName}.Data.Contracts.Dto;
using {solutionName}.Data.Contracts.Providers;
using {solutionName}.Data.Fake.Containers;
using {solutionName}.Data.Fake.ProviderBuilders;

namespace {solutionName}.Data.Fake.Providers
{{
    [UsedImplicitly]
    internal sealed class Fake{entityName}DataProvider : FakeProviderBase<{entityName}ProviderBuilder, I{entityName}DataProvider>, I{entityName}DataProvider
    {{
        private readonly Random _random = new Random();

        public Fake{entityName}DataProvider(
            {entityName}ProviderBuilder sampleProviderBuilder,
            I{entityName}DataContainer sampleContainer)
            : base(sampleProviderBuilder)
        {{
            sampleProviderBuilder.WithItems(sampleContainer.Items);
        }}

        IEnumerable<{entityName}Dto> I{entityName}DataProvider.GetItems() => GetService(r =>
        {{
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }}).GetItems();

        bool I{entityName}DataProvider.DeleteItem(Guid id) => GetService(r =>
        {{
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }}).DeleteItem(id);

        bool I{entityName}DataProvider.UpdateItem({entityName}Dto dto) => GetService(r =>
        {{
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }}).UpdateItem(dto);

        void I{entityName}DataProvider.CreateItem({entityName}Dto dto) => GetService(r =>
        {{
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }}).CreateItem(dto);
    }}
}}")).WithFolder($"{solutionName}.Data.Fake.Providers",
                    r => r.WithFile("Module.cs",
                        $@"using System;
using JetBrains.Annotations;
using {solutionName}.Data.Contracts.Dto;
using {solutionName}.Data.Contracts.Providers;
using {solutionName}.Data.Fake.Containers;
using {solutionName}.Data.Fake.ProviderBuilders;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace {solutionName}.Data.Fake.Providers
{{
    [UsedImplicitly]
    internal sealed class Module : ICompositionModule<IDependencyRegistrator>
    {{
        public void RegisterModule(IDependencyRegistrator dependencyRegistrator)
        {{
            dependencyRegistrator
                .AddInstance(Initialize{entityName}Container())
                .AddSingleton<I{entityName}DataProvider, Fake{entityName}DataProvider>();
            dependencyRegistrator.RegisterInstance({entityName}ProviderBuilder.CreateBuilder());
        }}

        private I{entityName}DataContainer Initialize{entityName}Container()
        {{
            var container = new {entityName}DataContainer();
            container.UpdateItems(new[]
            {{
                new {entityName}Dto
                {{
                    Id = Guid.NewGuid(),
                    DisplayName = ""PC"",
                    Value = 8
                }},

                new {entityName}Dto
                {{
                    Id = Guid.NewGuid(),
                    DisplayName = ""Acme"",
                    Value = 10
                }},

                new {entityName}Dto
                {{
                    Id = Guid.NewGuid(),
                    DisplayName = ""Bacme"",
                    Value = 3
                }},

                new {entityName}Dto
                {{
                    Id = Guid.NewGuid(),
                    DisplayName = ""Exceed"",
                    Value = 100
                }},

                new {entityName}Dto
                {{
                    Id = Guid.NewGuid(),
                    DisplayName = ""Acme2"",
                    Value = 10
                }}
            }});
            return container;
        }}
    }}
}}"));


            structure.AssertGeneratedCode();
        }
    }
}