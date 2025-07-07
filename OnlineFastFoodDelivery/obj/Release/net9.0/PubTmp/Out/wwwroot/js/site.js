
async function GetRecipe(recipeName,id,isAllShow    ) {
    let resp = await fetch(`${apiURL}?search=${recipeName}&key=${apiKey}`);
    let result = await resp.json();
    console.log(result);
    let Recipes = isAllShow ? result.data.recipes : result.data.recipes.slice(0, 6);
    showRecipes(Recipes, id);
}
function showRecipes(recipes, id) {
    $.ajax({
        contentType: "application/json;charset=utf-8",
        datatype: 'html',
        type: 'Post',
        url: '/Recipe/GetRecipeCard',
        data: JSON.stringify(recipes),
        success: function (htmlResult) { 
            $('#' + id).html(htmlResult);
        }
      })
}