﻿namespace Job.Domain.Commands.Rent;

public record CancelRentCommand(Guid Id, DateTime DatePreview);