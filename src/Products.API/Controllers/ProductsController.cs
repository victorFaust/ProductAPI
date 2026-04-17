using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Application.Common;
using Products.Application.Common.DTOs;
using Products.Application.Services;
namespace Products.API.Controllers
{

    [Authorize]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>Get all products, optionally filtered by colour.</summary>
        /// 
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ErrorResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProducts( [FromQuery] string? colour)
        {
            var result = string.IsNullOrWhiteSpace(colour)
                ? await _productService.GetAllAsync()
                : await _productService.GetByColourAsync(colour);

            return Ok(ApiResponse<IEnumerable<ProductDto>>.Success(result));
        }

        /// <summary>Create a new product.</summary>
        /// 
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<ValidationErrorResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ErrorResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateProduct( [FromBody] CreateProductRequest request)
        {
            var dto = new CreateProductDto(
                request.Name,
                request.Description ?? string.Empty,
                request.Price,
                request.Colour);

            var result = await _productService.CreateAsync(dto);

            var apiResponse = ApiResponse<ProductDto>.Success(result, "Product created", 201);
            return CreatedAtAction(nameof(GetProducts), new { id = result.Id }, apiResponse);
        }
        /// <summary>Update an existing product.</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ValidationErrorResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ErrorResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
        {
            var dto = new UpdateProductDto(
                request.Name,
                request.Description ?? string.Empty,
                request.Price,
                request.Colour);

            var result = await _productService.UpdateAsync(id, dto);
            return Ok(ApiResponse<ProductDto>.Success(result, "Product updated"));
        }

        /// <summary>Delete a product.</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<ErrorResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}