using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProduct productInterface) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
    {
        // Get All products from repo
        var products = await productInterface.GetAllAsync();
        if (!products.Any())
            return NotFound("No products detected in the database");

        // convert to dto list from entity and return
        var (_, list) = ProductConversion.FromEntity(null!, products);

        return list!.Any() ? Ok(list) : NotFound("No product found");
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDTO>> GetProduct(int id)
    {
        // Get single product from repo
        var product = await productInterface.FindByIdAsync(id);
        if (product == null)
            return NotFound("Product requested not found!");

        // convert to dto from entity and return
        var (_product, _) = ProductConversion.FromEntity(product, null!);

        return _product is not null ? Ok(_product) : NotFound("Product not found");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
    {
        //if(!User.IsInRole("Admin"))
        //    return BadRequest("This action is restricted!");

        // check model state is valid
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // convert to entity from dto and return
        var getEntity = ProductConversion.ToEntity(product);

        var response = await productInterface.CreateAsync(getEntity);
        return response.Flag is true ? Ok(response) : BadRequest(response);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
    {
        //if (!User.IsInRole("Admin"))
        //    return BadRequest("This action is restricted!");

        // check model state is valid
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // convert to entity from dto and return
        var getEntity = ProductConversion.ToEntity(product);

        var response = await productInterface.UpdateAsync(getEntity);
        return response.Flag is true ? Ok(response) : BadRequest(response);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
    {
        //if (!User.IsInRole("Admin"))
        //    return BadRequest("This action is restricted!");

        // convert to entity from dto and return
        var getEntity = ProductConversion.ToEntity(product);

        var response = await productInterface.DeleteAsync(getEntity);
        return response.Flag is true ? Ok(response) : BadRequest(response);
    }
}
