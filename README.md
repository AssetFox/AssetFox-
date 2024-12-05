# Welcome to PennDOT AssetFox

This application is designed to assist transportation agencies in determining where best to perform maintenance, rehabilitation, and replacement of their transportation assets, such as bridges and pavements, using a **Lowest Life Cycle Cost (LLCC)** approach. 

AssetFox is **asset-agnostic**, meaning an analysis can be performed on any asset, provided the following information is available:

- **Available Funds**
- **Condition data**
- **Deterioration Models**
- **Treatments and Consequences**

### Features

Once the required data is entered, the system can generate an optimal set of treatments based on:
  - Budget (none, entered or unlimited)
  - Minimum condition requirements
  - Incorporating existing committed projects 

AssetFox is an enterprise level **web application**, enabling:
- Roll-based user permissions
- User-level information sharing
- Data libraries
- Network details
  
---

## Architecture

AssetFox consists of the following components:

### 1. **Frontend**
- Built using **Vue.js**
- Located in the `BridgeCareApp/VuejsApp` folder
- Contains:
  - Frontend code
  - Dependency versions
  - Build scripts
- Interfaces with:
  - IAM provider
  - Backend API via **Axios**

### 2. **Backend**
- Built with **.NET**
- Located in folders starting with:
  - `BridgeCareApp/AppliedResearchAssociates`
  - `BridgeCareApp/BridgeCareCore`
- Includes:
  - Analysis and aggregation engines
  - Entity Framework database migrations
  - `Startup.cs` and `Program.cs`
  - API endpoints
  - Configuration files:
    - Connection strings
    - IAM role mappings
    - IAM-specific configurations

### 3. **Database**
- Uses **Microsoft SQL Server**
- Stores:
  - Network data uploads
  - Simulation libraries
  - Aggregated asset networks
  - Scenario outputs
  - User information
- Schema defined via **Entity Framework Core**
- Access restricted to the backend application

### 4. **Identity and Access Management (IAM)**
- Requires a third-party IAM service:
  - **Azure AD B2C** (for development)
  - Configured via `RolesToClaimsMapping.json` to map IAM roles to AssetFox-specific roles
  - No security option for testing and evaluation
---

## Setup

### Local Development
For local development setup, refer to the following link:  
[Setting Up the Development Environment](https://ara-cu.visualstudio.com/Infrastructure%20Asset%20Management/_wiki/wikis/Infrastructure-Asset-Management.wiki/268/Setting-Up-the-Development-Environment)

### Deployment
For deployment instructions, refer to the following link:  
[Deployment Information](https://docs.google.com/document/d/1OfNxmY_X4zqXfU0_XCQp30tmHm7vKTy8t30dyejdIrU/edit?usp=sharing)
