using FluentNHibernate.Mapping;

namespace Catalog.Shared.NHibernate.Mapping
{
  public class CategoryMap : SubclassMap<Model.Category>
  {
    public CategoryMap()
    {
      DiscriminatorValue("CCE18E89-939C-45E6-A13D-F66862EED71B");
    }
  }
}