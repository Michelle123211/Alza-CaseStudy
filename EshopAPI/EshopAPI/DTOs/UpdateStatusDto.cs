using EshopAPI.Infrastructure;

namespace EshopAPI.DTOs;

public class UpdateStatusDto {
	public string Status { get; set; }
	public string? Message { get; set; }

	public UpdateStatusDto(QueuedUpdateResult updateResult) {
		this.Status = updateResult.Status.ToString();
		this.Message = updateResult.Message;
	}
}