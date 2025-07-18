﻿using ExpenseTracker.Application.DTO.Expense;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Expense.Queries;

public record SearchExpensesWithIdQuery(Guid id, int pageSize) : IRequest<PagedResult<ExpenseListDTO>>;