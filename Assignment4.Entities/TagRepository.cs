using System.Collections.Generic;
using System.Linq;
using Assignment4.Core;

namespace Assignment4.Entities
{
    public class TagRepository : ITagRepository
    {
        private readonly KanbanContext _dbContext;

        public TagRepository(KanbanContext context)
        {
            _dbContext = context;
        }

        public (Response Response, int TagId) Create(TagCreateDTO tag)
        {
            Tag dbTag = _dbContext.Tags.SingleOrDefault(t => t.Name == tag.Name); //Trying to create a tag which exists already should return Conflict.
            if (dbTag != null)
            {
                return (Response.Conflict, dbTag.Id);
            }
            else
            {
                var newTag = new Tag
                {
                    Name = tag.Name,
                };

                _dbContext.Tags.Add(newTag);
                _dbContext.SaveChanges();
                return (Response.Created, newTag.Id);
            }
        }

        public IReadOnlyCollection<TagDTO> ReadAll()
        {
            return _dbContext.Tags.Select(x => new TagDTO(x.Id, x.Name)).ToList();
        }

        public TagDTO Read(int tagId)
        {
            Tag tag = _dbContext.Tags.SingleOrDefault(t => t.Id == tagId);
            if (tag == null)
            {
                return null;
            }
            else
            {
                return new TagDTO(tag.Id, tag.Name);
            }
        }

        public Response Update(TagUpdateDTO tag)
        {
            Tag dbTag = _dbContext.Tags.SingleOrDefault(x => x.Id == tag.Id);
            if (dbTag == null)
            {
                return Response.NotFound;
            }
            else
            {
                dbTag.Name = tag.Name;
                _dbContext.SaveChanges();
                return Response.Updated;
            }
        }

        public Response Delete(int tagId, bool force = false)
        {
            Tag tag = _dbContext.Tags.SingleOrDefault(t => t.Id == tagId);

            if (tag == null)
            {
                return Response.NotFound;
            }
            else if (tag.Tasks == null || force) //Tags which are assigned to a task may only be deleted using the force.
            {
                _dbContext.Tags.Remove(tag);
                _dbContext.SaveChanges();
                return Response.Deleted;
            }
            else
            {
                return Response.Conflict;
            }
        }
    }
}