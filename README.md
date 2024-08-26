Microsoft.AspNetCore.Components.WebAssembly.Rendering.WebAssemblyRenderer[100]
      Unhandled exception rendering component: The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true. Path: $ | LineNumber: 0 | BytePositionInLine: 0.
System.Text.Json.JsonException: The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true. Path: $ | LineNumber: 0 | BytePositionInLine: 0.
 ---> System.Text.Json.JsonReaderException: The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true. LineNumber: 0 | BytePositionInLine: 0.
   at System.Text.Json.ThrowHelper.ThrowJsonReaderException(Utf8JsonReader& json, ExceptionResource resource, Byte nextByte, ReadOnlySpan`1 bytes)
   at System.Text.Json.Utf8JsonReader.Read()
   at System.Text.Json.Serialization.JsonConverter`1[[InsuranceApi.DTOs.PolicyDto, SharedModels, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].ReadCore(Utf8JsonReader& reader, JsonSerializerOptions options, ReadStack& state)
   --- End of inner exception stack trace ---
   at System.Text.Json.ThrowHelper.ReThrowWithPath(ReadStack& state, JsonReaderException ex)
   at System.Text.Json.Serialization.JsonConverter`1[[InsuranceApi.DTOs.PolicyDto, SharedModels, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].ReadCore(Utf8JsonReader& reader, JsonSerializerOptions options, ReadStack& state)
   at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1[[InsuranceApi.DTOs.PolicyDto, SharedModels, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].ContinueDeserialize(ReadBufferState& bufferState, JsonReaderState& jsonReaderState, ReadStack& readStack)
   at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.<DeserializeAsync>d__1[[InsuranceApi.DTOs.PolicyDto, SharedModels, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].MoveNext()
   at System.Net.Http.Json.HttpContentJsonExtensions.<ReadFromJsonAsyncCore>d__12`1[[InsuranceApi.DTOs.PolicyDto, SharedModels, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].MoveNext()
   at ClientApp.Services.PolicyDtoService.Add(PolicyDto employee) in C:\Users\6147958\source\repos\FnfProject\ClientApp\Services\PolicyService.cs:line 37
   at ClientApp.Pages.AfterLogin.PlansDetail.HandleValidSubmit() in C:\Users\6147958\source\repos\FnfProject\ClientApp\Pages\AfterLogin\PlansDetail.razor:line 170
   at Microsoft.AspNetCore.Components.ComponentBase.CallStateHasChangedOnAsyncCompletion(Task task)
   at Microsoft.AspNetCore.Components.Forms.EditForm.HandleSubmitAsync()
   at Microsoft.AspNetCore.Components.ComponentBase.CallStateHasChangedOnAsyncCompletion(Task task)
   at Microsoft.AspNetCore.Components.RenderTree.Renderer.GetErrorHandledTask(Task taskToHandle, ComponentState owningComponentState)
