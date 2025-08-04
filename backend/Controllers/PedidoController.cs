using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly PedidoContext _context;

        public OrdersController(PedidoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            return await _context.Pedidos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound();
            return pedido;
        }

        [HttpPost]
        public async Task<ActionResult<Pedido>> CreatePedido(Pedido pedido)
        {
            pedido.Status = "Pendente";
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
            
            IMessenger messenger = new RabbitMQMessenger();
            messenger.SendMessage(pedido.Id.ToString());

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePedido(int id, Pedido pedido)
        {
            if (id != pedido.Id)
                return BadRequest();
            _context.Entry(pedido).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Pedidos.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound();
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("ChangeStatusToProcessando/{id}")]
        public async Task<IActionResult> ChangeStatusToProcessando(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound();

            pedido.Status = "Processando";
            await _context.SaveChangesAsync();

            return Ok(pedido);
        }

        [HttpGet("ChangeStatusToFinalizado/{id}")]
        public async Task<IActionResult> ChangeStatusToFinalizado(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound();

            pedido.Status = "Finalizado";
            await _context.SaveChangesAsync();

            return Ok(pedido);
        }

    }





    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        public TestController()
        {

        }

        

        [HttpGet]
        public async Task<ActionResult<string>> GetTest()
        {
            return await GetTest(0);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetTest(int id)
        {
            try
            {
                IMessenger messenger = new RabbitMQMessenger();
                messenger.SendMessage(id.ToString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return "Ok! " + id;
        }
    }
}
