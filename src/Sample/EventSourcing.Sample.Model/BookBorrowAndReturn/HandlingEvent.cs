﻿using System;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.Model.BookBorrowAndReturn
{
    public class HandlingEvent : AggregateRoot<Guid>
    {
        public HandlingEvent() { }
        public HandlingEvent(Book book, LibraryAccount account, Library library, HandlingType handlingType) : base(Guid.NewGuid())
        {
            Assert.IsValid(book);
            Assert.IsValid(account);
            Assert.IsValid(library);
            OnEvent(new HandlingEventCreated(Id, book.Id, account.Id, library.Id, handlingType, DateTime.Now));
        }

        public Guid BookId { get; private set; }
        public Guid AccountId { get; private set; }
        public Guid LibraryId { get; private set; }
        public HandlingType HandlingType { get; private set; }
        public DateTime Time { get; private set; }
    }
    public enum HandlingType
    {
        Borrow = 1,
        Return
    }
    [Event]
    public class HandlingEventCreated
    {
        public Guid Id { get; private set; }
        public Guid BookId { get; private set; }
        public Guid AccountId { get; private set; }
        public Guid LibraryId { get; private set; }
        public HandlingType HandlingType { get; private set; }
        public DateTime Time { get; private set; }

        public HandlingEventCreated(Guid id, Guid bookId, Guid accountId, Guid libraryId, HandlingType handlingType, DateTime time)
        {
            Id = id;
            BookId = bookId;
            AccountId = accountId;
            LibraryId = libraryId;
            HandlingType = handlingType;
            Time = time;
        }
    }
}
