# Break-and-fix exercises

Start from a working local deployment. Apply one fault, diagnose it before reading the cause, fix it, and write down what you saw.

Useful commands:

```sh
kubectl -n playground get pods,svc,endpointslices
kubectl -n playground describe pod <pod>
kubectl -n playground logs <pod> --previous
kubectl -n playground get events --sort-by=.lastTimestamp
helm -n playground history playground-api
helm -n playground rollback playground-api <revision>
```

Base command, called `UPGRADE` below:

```sh
helm upgrade --install playground-api deploy/helm/playground-api -n playground
```

| # | Inject the fault | Investigate |
|---|---|---|
| 1 | `UPGRADE --set image.tag=nope` | Why do new pods never start? Why does the API still answer? |
| 2 | Change the readiness path in `templates/deployment.yaml` to `/health/nope`, then `UPGRADE` | Pods run but are not Ready. What does the rollout do? |
| 3 | Change the liveness path the same way | What happens to the restart count? |
| 4 | `UPGRADE --set resources.requests.memory=8Mi --set resources.limits.memory=16Mi` | What is the termination reason? |
| 5 | `UPGRADE --set resources.requests.cpu=64` | Why are pods Pending? |
| 6 | Add a wrong label to the selector in `templates/service.yaml`, then `UPGRADE` | Pods are Ready but requests fail. Check the endpoints. |

Revert file edits with `git checkout -- deploy/` and run `UPGRADE` again.

## Notes template

- **Symptom:**
- **Commands that found it:**
- **Root cause:**
- **Fix:**
- **How a pipeline or alert could catch it earlier:**
