﻿using Microsoft.EntityFrameworkCore;
using MediatR;
using AutoMapper;
using C2IEDM.Web.Persistence;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.ObjectItems.DTOs;

namespace C2IEDM.Web.Application.ObjectItems;

public enum ObjectItemCategory
{
    ObjectItem,
    Organisation,
    Unit
}

public class List
{
    public class Query : IRequest<Result<PagedList<ObjectItemDto>>>
    {
        public ObjectItemCategory Category { get; set; }
        public PagingParams Params { get; set; }
        public DateTime? TimeOfInterest { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<ObjectItemDto>>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<ObjectItemDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = request.Category switch
            {
                ObjectItemCategory.Unit => _context.Units.AsQueryable(),
                ObjectItemCategory.Organisation => _context.Organisations.AsQueryable(),
                ObjectItemCategory.ObjectItem => _context.ObjectItems.AsQueryable(),
                _ => null
            };

            query = request.TimeOfInterest.HasValue
                ? query.Where(_ => _.Created < request.TimeOfInterest.Value && _.Superseded > request.TimeOfInterest.Value)
                : query.Where(_ => _.Superseded == DateTime.MaxValue);

            var count = await query.CountAsync();

            var objectItems = await query
                .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
                .Take(request.Params.PageSize)
                .ToListAsync();

            var objectItemDtos = objectItems.Select(_ =>
            {
                var temp = _mapper.Map(_, _.GetType(), typeof(ObjectItemDto));

                if (temp is not ObjectItemDto objectItemDto)
                {
                    throw new InvalidDataException();
                }

                return objectItemDto;
            });

            return Result<PagedList<ObjectItemDto>>.Success(
                new PagedList<ObjectItemDto>(objectItemDtos, count, request.Params.PageNumber, request.Params.PageSize));
        }
    }
}