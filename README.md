# devops-playground

A small .NET 10 API used to practise containers, Kubernetes, Helm, CI/CD, security scanning and Terraform on Azure.

## Layout

| Path | What it is |
|---|---|
| `src/`, `tests/` | Minimal API and its integration tests |
| `Dockerfile` | Multi-stage build, non-root chiseled runtime |
| `deploy/helm/` | Helm chart with probes, resource limits and a locked-down security context |
| `deploy/kind/` | Local two-node cluster |
| `infra/terraform/` | VNet, AKS, ACR, Key Vault and workload identity |
| `.github/` | CI (test, scan, deploy to kind), CodeQL (runs only if the repo is public), Dependabot |
| `docs/troubleshooting.md` | Break-and-fix exercises |

## Run locally (free)

You need Docker Desktop, [kind](https://kind.sigs.k8s.io/), kubectl and Helm.

```sh
docker build -t playground-api:local .
kind create cluster --name playground --config deploy/kind/cluster.yaml
kind load docker-image playground-api:local --name playground
helm upgrade --install playground-api deploy/helm/playground-api --namespace playground --create-namespace --wait
kubectl -n playground port-forward service/playground-api 8080:80
```

Then, in another terminal:

```sh
curl http://localhost:8080/api/items
```

Clean up with `kind delete cluster --name playground`.

Run the tests with `dotnet test Playground.slnx` (needs the .NET 10 SDK).

## Check the Terraform (free)

```sh
cd infra/terraform
terraform fmt -check
terraform init -backend=false
terraform validate
```

After the first `init`, commit `.terraform.lock.hcl` so provider versions stay fixed.

## Deploy to Azure (costs credit)

1. `az login`, then set the `ARM_SUBSCRIPTION_ID` environment variable.
2. Check your vCPU quota for the node size in `variables.tf`.
3. Copy `terraform.tfvars.example` to `terraform.tfvars` and add your public IP.
4. `terraform init`, `terraform plan -out tfplan`, `terraform apply tfplan`.
5. Push the image: `az acr login --name <registry>`, then `docker tag` and `docker push`.
6. `az aks get-credentials --resource-group <rg> --name <aks>`.
7. `helm upgrade --install` as above, adding `--set image.repository=<login server>/playground-api`, `--set image.tag=<tag>` and `--set workloadIdentity.clientId=<client id>`.
8. **`terraform destroy` when you finish.**

State is kept locally and is git-ignored.

## Not done yet

- The API does not read from Key Vault; the identity and role are ready for it.
- The pipeline deploys to kind only. Deploying to AKS with GitHub OIDC is the next step.
