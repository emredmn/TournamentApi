# Tournament API

A simple GraphQL API for managing knockout tournaments, built with ASP.NET Core and HotChocolate.

## Features
- **User Management**: Register and Login with JWT Authentication.
- **Tournament Management**: Create tournaments, add participants, and automatic bracket generation.
- **Match Management**: Play matches and query match history.
- **Security**: Role-Based Authorization (JWT).

## Setup & Running

1. **Prerequisites**: .NET 8.0 SDK.
2. **Run the Project**:
   ```powershell
   cd TournamentApi
   dotnet run
   ```
3. **Access GraphQL IDE**:
   Open functionality is available at: [http://localhost:5224/graphql/](http://localhost:5224/graphql/)

---

## Step-by-Step Testing Guide

Follow these steps in the **Banana Cake Pop** (GraphQL IDE) to verify the application.

### 1. Register Users
Create two different users to play against each other.

```graphql
mutation RegisterUser1 {
  register(
    email: "user1@example.com",
    password: "password123",
    firstName: "Ali",
    lastName: "Veli"
  )
}
```
*Run again with different details for User 2 (e.g., `user2@example.com`).*

### 2. Login & Get Token
Login as **User 1** to get your Access Token.

```graphql
mutation Login {
  login(
    email: "user1@example.com", 
    password: "password123"
  )
}
```
> **IMPORTANT:** Copy the returned token (without quotes).
> In Banana Cake Pop, go to the **Gear Icon (Settings)** -> **HTTP Headers** and add:
> - **Name:** `Authorization`
> - **Value:** `Bearer <YOUR_TOKEN_HERE>`

### 3. Create Tournament
```graphql
mutation CreateTournament {
  createTournament(name: "Champions Cup") {
    id
    name
    status
  }
}
```
*Note the Tournament ID (e.g., `1`).*

### 4. Add Participants
Add both users to the tournament (Assuming Tournament ID is 1, User IDs are 1 and 2).

```graphql
mutation JoinTournament {
  add1: addParticipant(tournamentId: 1, userId: 1)
  add2: addParticipant(tournamentId: 1, userId: 2)
}
```

### 5. Start Tournament
Generates the bracket and matches.

```graphql
mutation Start {
  startTournament(tournamentId: 1) {
    id
    status
    bracket {
      id
      matches {
        id
        player1 { firstName }
        player2 { firstName }
      }
    }
  }
}
```

### 6. Play a Match
Simulate User 1 winning Match 1.

```graphql
mutation PlayMatch {
  playMatch(matchId: 1, winnerId: 1) {
    id
    winner {
      firstName
      lastName
    }
  }
}
```

### 7. Get My Matches
Verify the core requirement: *Fetch my matches without sending my ID*.

```graphql
query GetMyMatches {
  myMatches {
    id
    round
    player1 { firstName lastName }
    player2 { firstName lastName }
    winner {
      firstName
      lastName
    }
  }
}
```
This should return the history of matches for the logged-in user.
