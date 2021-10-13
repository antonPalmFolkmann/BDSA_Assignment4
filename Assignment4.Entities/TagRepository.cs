using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;
using System;

namespace Assignment4.Entities
{
    public class TagRepository : ITagRepository
    {
        private readonly KanbanContext _context;

        public TagRepository(KanbanContext context)
        {
            _context = context;
        }

        public (Response Response, int TagId) Create(TagCreateDTO tag)
        {
            var tagsWithSameName = from c in _context.Tags
                                    where c.Name == tag.Name
                                    select new TagDTO(
                                        c.Id,
                                        c.Name
                                    );
            

            if (tagsWithSameName.Count() != 0){
                return (Response.Conflict, -1);
            } 

            var entity = new Tag{Name = tag.Name};

            _context.Tags.Add(entity);

            _context.SaveChanges();

            return (Response.Created, entity.Id);
        }

        public Response Delete(int tagId, bool force = false)
        {
            var entity = _context.Tags.Find(tagId);

            _context.Tags.Remove(entity);
            _context.SaveChanges();

            return Response.Deleted;
        }

        public TagDTO Read(int tagId)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<TagDTO> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public Response Update(TagUpdateDTO tag)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
