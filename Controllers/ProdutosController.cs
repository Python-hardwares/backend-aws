using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TB2_aws.Data;
using TB2_aws.Models;
using Microsoft.AspNetCore.Authorization;

namespace TB2_aws.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/produtos
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var produtos = await _context.Produtos.ToListAsync();
        return Ok(produtos);
    }

    // GET: api/produtos/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto == null)
            return NotFound();

        return Ok(produto);
    }

    // POST: api/produtos
    [HttpPost]
    public async Task<IActionResult> Post(Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = produto.Id },
            produto
        );
    }

    // PUT: api/produtos/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Produto produto)
    {
        if (id != produto.Id)
            return BadRequest("ID da URL diferente do ID do produto.");

        var produtoExistente = await _context.Produtos.FindAsync(id);

        if (produtoExistente == null)
            return NotFound();

        produtoExistente.Nome = produto.Nome;
        produtoExistente.Preco = produto.Preco;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/produtos/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto == null)
            return NotFound();

        _context.Produtos.Remove(produto);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
