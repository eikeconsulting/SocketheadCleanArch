using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sockethead.Razor.Alert.Extensions;

namespace SocketheadCleanArch.Admin.Extensions;

public static class ValidationExtension
{
    public static ValidationResult AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
    {
        foreach (var error in result.Errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return result;
    }
    
    public static IActionResult ApplyValidation(
        this IActionResult ar, 
        ValidationResult result, 
        ModelStateDictionary modelState, 
        string errorMessage)
    {
        result.AddToModelState(modelState);
        return ar.Error(errorMessage);
    }
}