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
}}")).WithFolder($"{solutionName}.Model",
                    r => r.WithFile($"{entityName}Service.cs",
                        $@"using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LogoFX.Client.Core;
using LogoFX.Core;
using {solutionName}.Data.Contracts.Providers;
using {solutionName}.Model.Contracts;
using {solutionName}.Model.Mappers;

namespace {solutionName}.Model
{{
    [UsedImplicitly]
    internal sealed class {entityName}Service : NotifyPropertyChangedBase<{entityName}Service>, I{entityName}Service
    {{
        private readonly I{entityName}DataProvider _provider;
        private readonly {entityName}Mapper _mapper;

        private readonly RangeObservableCollection<I{entityName}> _items =
            new RangeObservableCollection<I{entityName}>();

        public {entityName}Service(I{entityName}DataProvider provider, {entityName}Mapper mapper)
        {{
            _provider = provider;
            _mapper = mapper;
        }}

        private void GetItems()
        {{
            var items = _provider.GetItems().Select(_mapper.MapTo{entityName});
            _items.Clear();
            _items.AddRange(items);
        }}

        IEnumerable<I{entityName}> I{entityName}Service.Items => _items;

        Task I{entityName}Service.GetItems() => MethodRunner.RunAsync(GetItems);

        Task<I{entityName}> I{entityName}Service.NewItem() => MethodRunner.RunWithResultAsync<I{entityName}>(() => new {entityName}());

        Task I{entityName}Service.SaveItem(I{entityName} item) => MethodRunner.RunAsync(() =>
        {{
            var dto = _mapper.MapTo{entityName}Dto(item);

            if (item.IsNew)
            {{
                _provider.CreateItem(dto);
            }}
            else
            {{
                _provider.UpdateItem(dto);
            }}
        }});

        Task I{entityName}Service.DeleteItem(I{entityName} item) => MethodRunner.RunAsync(() =>
        {{
            _provider.DeleteItem(item.Id);
            _items.Remove(item);
        }});
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
}}")).WithFolder($"{solutionName}.Data.Real.Providers",
                    r => r.WithFile($"{entityName}DataProvider.cs",
                        $@"using System;
using System.Collections.Generic;
using {solutionName}.Data.Contracts.Dto;
using {solutionName}.Data.Contracts.Providers;

namespace {solutionName}.Data.Real.Providers
{{
    internal sealed class {entityName}DataProvider : I{entityName}DataProvider
    {{
        public IEnumerable<{entityName}Dto> GetItems()
        {{
            throw new NotImplementedException();
        }}

        public bool DeleteItem(Guid id)
        {{
            throw new NotImplementedException();
        }}

        public bool UpdateItem({entityName}Dto dto)
        {{
            throw new NotImplementedException();
        }}

        public void CreateItem({entityName}Dto dto)
        {{
            throw new NotImplementedException();
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
}}"));
            structure.AssertGeneratedCode();
        }
    }
}