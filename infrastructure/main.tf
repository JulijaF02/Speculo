# =============================================================================
# SPECULO INFRASTRUCTURE AS CODE (Terraform)
# Provisions: RG, ACR, AKS, Managed PostgreSQL, Log Analytics
# =============================================================================

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
  }
  subscription_id = var.subscription_id
}

# --- VARIABLES ---
variable "subscription_id" {
  description = "Azure subscription ID to deploy into"
  type        = string
}

variable "location" {
  default = "West US 2"
}

variable "resource_group_name" {
  default = "speculo-rg"
}

# --- COMMON RESOURCES ---
resource "random_string" "suffix" {
  length  = 6
  special = false
  upper   = false
}


resource "azurerm_resource_group" "speculo_rg" {
  name     = var.resource_group_name
  location = var.location
  tags     = { Project = "Speculo", ManagedBy = "Terraform" }
}

# --- 1. CONTAINER REGISTRY (ACR) ---
resource "azurerm_container_registry" "speculo_acr" {
  name                = "speculoacr${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.speculo_rg.name
  location            = azurerm_resource_group.speculo_rg.location
  sku                 = "Basic"
  admin_enabled       = true
}

# --- 2. KUBERNETES SERVICE (AKS) ---
resource "azurerm_kubernetes_cluster" "speculo_aks" {
  name                = "speculo-aks"
  location            = azurerm_resource_group.speculo_rg.location
  resource_group_name = azurerm_resource_group.speculo_rg.name
  dns_prefix          = "speculoaks"
  sku_tier            = "Free"

  default_node_pool {
    name       = "default"
    node_count = 1
    vm_size    = "Standard_B2s_v2"
  }

  identity {
    type = "SystemAssigned"
  }
}

# AKS -> ACR pull access
resource "azurerm_role_assignment" "aks_acr_pull" {
  principal_id                     = azurerm_kubernetes_cluster.speculo_aks.kubelet_identity[0].object_id
  role_definition_name             = "AcrPull"
  scope                            = azurerm_container_registry.speculo_acr.id
  skip_service_principal_aad_check = true
}


# --- 4. LOGGING & MONITORING ---
resource "azurerm_log_analytics_workspace" "speculo_logs" {
  name                = "law-speculo-${random_string.suffix.result}"
  location            = azurerm_resource_group.speculo_rg.location
  resource_group_name = azurerm_resource_group.speculo_rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_application_insights" "speculo_insights" {
  name                = "ai-speculo-${random_string.suffix.result}"
  location            = azurerm_resource_group.speculo_rg.location
  resource_group_name = azurerm_resource_group.speculo_rg.name
  workspace_id        = azurerm_log_analytics_workspace.speculo_logs.id
  application_type    = "web"
}

# --- OUTPUTS ---
output "resource_group_name" {
  value = azurerm_resource_group.speculo_rg.name
}

output "acr_login_server" {
  value = azurerm_container_registry.speculo_acr.login_server
}

output "acr_name" {
  value = azurerm_container_registry.speculo_acr.name
}

output "aks_cluster_name" {
  value = azurerm_kubernetes_cluster.speculo_aks.name
}


output "app_insights_key" {
  value     = azurerm_application_insights.speculo_insights.instrumentation_key
  sensitive = true
}
