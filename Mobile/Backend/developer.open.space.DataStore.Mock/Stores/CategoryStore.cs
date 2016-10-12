using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Mock.Stores
{
    public class CategoryStore : BaseStore<Category>, ICategoryStore
    {
        public override Task<IEnumerable<Category>> GetItemsAsync(bool forceRefresh = false)
        {
            var categories = new[]
                 {
                    new Category { Name = "MobileDevleopment", ShortName="Mobile", Color="#B8E986"},
                    new Category { Name = "Testing", ShortName="Testing", Color="#F16EB0"},
                    new Category { Name = "Frameworks", ShortName="FX", Color="#7DD5C9" },
                    new Category { Name = "Tools", ShortName="Tools", Color="#51C7E3"},
                    new Category { Name = "Requirements Management", ShortName="Requirements", Color="#F88F73" },
                    new Category { Name = "Gaming", ShortName="Gaming", Color="#4B637E"},
                    new Category { Name = "WebDevelopment", ShortName="Web", Color="#AC9AD3" },
                    new Category { Name = "Internet of Things", ShortName="IoT", Color="#AC9AD3" },
                    new Category { Name = "Agile software development", ShortName="Agile", Color="#AC9AD3" },
                    new Category { Name = "Legal and Ethics", ShortName="Legal", Color="#AC9AD3" },
                    new Category { Name = "Patterns", ShortName="Patterns", Color="#AC9AD3" },
                    new Category { Name = "Artificial Intelligence", ShortName="AI", Color="#AC9AD3" },
                    new Category { Name = "Programming Languages", ShortName="Languages", Color="#AC9AD3" },
                };
            return Task.FromResult(categories as IEnumerable<Category>);
        }
    }
}
