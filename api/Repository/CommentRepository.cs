using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {   

        private readonly ApplicationDBContext _context;
        public CommentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> Delete(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if(comment == null){
                return null;
            }
            _context.Remove(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments.Include(a => a.appUser).ToListAsync();
        }

      
        public async Task<Comment?> GetByIdAsync(int id)
        {
        return await _context.Comments.Include(a => a.appUser).FirstOrDefaultAsync(c=>c.Id == id);

        }

        public async Task<Comment?> UpdateAsync(int id, UpdateCommentDto commentDto)
        {
           var comment = await _context.Comments.FirstOrDefaultAsync(c=> c.Id == id);
           if(comment == null){
            return null;
           }

           comment.Title = commentDto.Title;
           comment.Content = commentDto.Content;

            await _context.SaveChangesAsync();
            return comment;
        }
    }
}