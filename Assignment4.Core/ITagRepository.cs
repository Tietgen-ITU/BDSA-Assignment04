using System.Collections.Generic;

namespace Assignment4.Core
{
    public interface ITagRepository
    {
        (Response Response, int TagId) Create(TagCreateDTO tag);
        IReadOnlyCollection<TagDTO> ReadAll();
        TagDTO Read(int tagId);
        Response Update(TagUpdateDTO tag);
        Response Delete(int tagId, bool force = false);
    }
}