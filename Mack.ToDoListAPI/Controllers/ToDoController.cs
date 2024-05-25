using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mack.ToDoListAPI.Controllers;
 

    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private static readonly List<ToDoItem> ToDoItems = new List<ToDoItem>();

        private readonly ILogger<ToDoController> _logger;

        public ToDoController(ILogger<ToDoController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            // Problema 1: Vulnerabilidade de segurança - exposição de dados sensíveis
            LogSensitiveData();

            return ToDoItems;
        }

        [HttpPost]    
        [ProducesResponseType<int>(StatusCodes.Status200k)]
        public IActionResult Post(ToDoItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            // Problema 2: Falta de verificação de duplicação
            ToDoItems.Add(item);

            // Problema 3: Falta de sanitização de entrada do usuário (possível injeção de SQL)
            if (!IsValidTitle(item.Title))
            {
                return BadRequest("Invalid title");
            }

            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Problema 4: Manipulação direta de dados sem verificações
            var item = ToDoItems.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            ToDoItems.Remove(item);
            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, ToDoItem newItem)
        {
            // Problema 5: Falta de validação de entrada (possível crash)
            var oldItem = ToDoItems.FirstOrDefault(x => x.Id == id);
            if (oldItem == null)
            {
                return NotFound();
            }

            oldItem.Title = newItem.Title;
            oldItem.IsComplete = newItem.IsComplete;

            // Linhas duplicadas
            oldItem.Title = newItem.Title;
            oldItem.IsComplete = newItem.IsComplete;

            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            // Problema 6: Vulnerabilidade de segurança - informações sensíveis expostas
            var item = ToDoItems.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            // Linhas duplicadas
            var duplicateItem = ToDoItems.FirstOrDefault(x => x.Id == id);
            if (duplicateItem == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        private void LogSensitiveData()
        {
            // Problema 7: Log de dados sensíveis
            _logger.LogInformation("Sensitive data: {0}", ToDoItems);

            // Linhas duplicadas
            _logger.LogInformation("Sensitive data: {0}", ToDoItems);
            

        }

        private bool IsValidTitle(string title)
        {
            // Problema 8: Validação inadequada (apenas exemplo fictício)
            return !title.Contains("DROP");
        }
    }

    public class ToDoItem
    {
        // Problema 9: Falta de validação de propriedades
        public int Id { get; set; }

        // Problema 10: Dados sensíveis podem ser expostos
        public string Title { get; set; }

        public bool IsComplete { get; set; }
    }
