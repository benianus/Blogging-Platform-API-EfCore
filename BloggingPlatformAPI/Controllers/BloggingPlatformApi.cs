using BloggingPlatformAPI.Data;
using BloggingPlatformAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloggingPlatformAPI.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class BloggingPlatformApi : ControllerBase
    {
        public BloggingPlatform _db { get; set; }
        public BloggingPlatformApi(BloggingPlatform db)
        {
            _db = db;
        }

        [HttpGet(Name = "GetAllPosts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<BlogPost>>> GetAllPosts()
        {
            var posts = await _db.BlogPost.ToListAsync();

            if (posts != null)
            {
                return Ok(posts);
            }

            return NotFound("Posts Not Found");
        }
        [HttpGet("{id}", Name = "GetPostById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BlogPost>> GetPostById(int id)
        {
            var posts = await _db.BlogPost.ToListAsync();
            var post = posts.Find(post => post.Id == id);

            if (post != null)
            {
                return Ok(post);
            }

            return NotFound("Posts Not Found");
        }
        [HttpGet("filter-by/{term}", Name = "FilterPostByTerm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<BlogPost>>> FilterPostsByTerm(string term)
        {
            var posts = await _db.BlogPost.ToListAsync();

            List<BlogPost>? filteredPosts = null;

            if (posts != null)
            {
                filteredPosts = posts.FindAll(p =>
                    p.Title.ToLower().Contains(term.ToLower()) ||
                    p.content.ToLower().Contains(term.ToLower()) ||
                    p.Category.ToLower().Contains(term.ToLower()) ||
                    p.Tags.ToLower().Contains(term.ToLower())
                );

                if (filteredPosts != null)
                {
                    return Ok(filteredPosts);
                }
            }

            return NotFound("Posts not found");
        }
        [HttpPost(Name = "CreatePost")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BlogPost>> CreatePost(BlogPost post)
        {
            if (post == null)
            {
                return BadRequest("Body empty");
            }

            await _db.BlogPost.AddAsync(post);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetPostById", new { Id = post.Id }, post);
        }

        [HttpPut("{id}", Name = "EditPost")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BlogPost>> UpdatePost(int id,[FromBody] BlogPost blogPost)
        {
            var findedPosts = await _db.BlogPost.ToListAsync();

            BlogPost? post = null;

            if (findedPosts != null)
            {
                post = findedPosts.Find(p => p.Id == id);

                if (post != null)
                {
                    post.Title = blogPost.Title;
                    post.content = blogPost.content;
                    post.Category = blogPost.Category;
                    post.Tags = blogPost.Tags;
                    post.UpdatedAt = DateTime.Now;

                    _db.BlogPost.Update(post);
                    await _db.SaveChangesAsync();

                    return CreatedAtRoute("GetPostById", new {Id = id} , post);
                }
            }

            return BadRequest("Bad request");
        }
        [HttpDelete("{id}", Name = "DeletePost")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> DeletePost(int id)
        {
            var posts = await _db.BlogPost.ToListAsync();
            var post = posts.Find(p => p.Id == id);

            if (post == null)
            {
                return NotFound("Post Not found");
            }

            _db.BlogPost.Remove(post);
            await _db.SaveChangesAsync();

            return Ok("Post Deleted Successfully");
        }
    }
}
