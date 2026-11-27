variable "location" {
  description = "Azure region."
  type        = string
  default     = "australiaeast"
}

variable "prefix" {
  description = "Short lowercase prefix used in resource names."
  type        = string
  default     = "pg"

  validation {
    condition     = can(regex("^[a-z][a-z0-9]{1,7}$", var.prefix))
    error_message = "Use 2 to 8 lowercase letters or digits, starting with a letter."
  }
}

variable "admin_ip_ranges" {
  description = "Public CIDR ranges allowed to reach the AKS API server and Key Vault."
  type        = list(string)

  validation {
    condition     = length(var.admin_ip_ranges) > 0 && !contains(var.admin_ip_ranges, "0.0.0.0/0")
    error_message = "Provide at least one range, and do not open access to the whole internet."
  }
}

variable "node_count" {
  description = "Number of AKS nodes. Keep within your vCPU quota."
  type        = number
  default     = 1
}

variable "vm_size" {
  description = "AKS node size. Check that your subscription has quota for this family."
  type        = string
  default     = "Standard_D2s_v5"
}

variable "namespace" {
  description = "Kubernetes namespace the API is deployed to."
  type        = string
  default     = "playground"
}
