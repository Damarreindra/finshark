using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
   [Route("api/comment")]
   [ApiController]
   public class CommentController : ControllerBase
   {
      private readonly ICommentRepository _commentRepo;
      private readonly UserManager<AppUser> _userManager;
      private readonly IStockRepository _stockRepo;

      public CommentController(ICommentRepository commentRepository, IStockRepository stockRepository, UserManager<AppUser> userManager)
      {
         _commentRepo = commentRepository;
         _stockRepo = stockRepository;
         _userManager = userManager;
      }

      [HttpGet]
      public async Task<IActionResult> GetAll()
      {
         if (!ModelState.IsValid)
            return BadRequest(ModelState);
         var comments = await _commentRepo.GetAllAsync();
         var commentDto = comments.Select(s => s.ToCommentDto());
         return Ok(commentDto);
      }

      [HttpGet("{id:int}")]
      public async Task<IActionResult> GetById([FromRoute] int id)
      {
         if (!ModelState.IsValid)
            return BadRequest(ModelState);
         var comment = await _commentRepo.GetByIdAsync(id);
         if (comment == null)
         {
            return NotFound();
         }
         return Ok(comment.ToCommentDto());
      }

      [HttpPost("{stockId:int}")]
      public async Task<IActionResult> Create([FromRoute] int stockId, CommentDto commentDto)
      {
         if (!ModelState.IsValid)
            return BadRequest(ModelState);
         if (!await _stockRepo.StockExist(stockId))
         {
            return BadRequest("Stock does not exist sad :(");
         }
         var username = User.GetUsername();
         var appUser = await _userManager.FindByNameAsync(username);

         var commentModel = commentDto.ToCommentCreateDto(stockId);
         commentModel.AppUserId = appUser.Id;

         await _commentRepo.CreateAsync(commentModel);
         return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
      }

      [HttpPut]
      [Route("{id:int}")]
      public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentDto updateComment)
      {
         if (!ModelState.IsValid)
            return BadRequest(ModelState);
         var comment = await _commentRepo.UpdateAsync(id, updateComment);
         if (comment == null)
         {
            return NotFound();
         }
         return Ok(comment.ToCommentDto());
      }

      [HttpDelete("{id}")]
      public async Task<IActionResult> Delete([FromRoute] int id)
      {

         await _commentRepo.Delete(id);
         return NoContent();
      }

   }
}