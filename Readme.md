## Instructions for use

##### Notes

Readme assumes you have created a Docker Hub account and used `docker login` to establish the
connection to Docker for various docker `push` commands.

Spec files should still work since current images references are public

#### Steps

1. `cd` into _Infrastructure_ and run the following commands
   _ `kubectl apply -f helm-rbac.yaml -f kube-demo.yaml
   _ `helm init`
   _ `cat RabbitMQ-Helm.txt | bash`
   _ Note this command will fail if `tiller` is not ready
   _ You can view the status of the setup through the K8s dashboard. Installation is not immediate
   _ Upon finishing you can use the Kubernetes dashboard to access the Service resouces in the _rabbit_ namespace. This will show you exposed ports on your node. The bottom one is the admin interface \* Use rabbit / rabbit to login

##### Deploy Price Generator

2. `cd` into _PriceGenerator_
3. Build the Docker image using the following command
   - `docker build -t <my-repo-name>/pg-demo:v1 .`
4. Push the created Docker image to your registry (assumes Docker Hub)
   - `docker push <my-repo-name>/pg-demo:v1`
5. Create the _Deployment_ within Kubernetes which will create the appropriate Pod
   - `kubectl apply -f ../Infrastructure/pricegenerator-spec.yaml`
6. Use the Kubernetes Dashboard to view the _kube-demo_ namespace. When the status is green you can check the RabbitMQ dashboard and see data coming in

##### Deploy Stock Web Api

7. `cd` into _StockPriceApi_ and execute the following Docker build command
   - `docker build -t <my-repo-name>/api-demo:v1 .`
8. Push the created image to Docker Hub
   - `docker push <my-repo-name>/api-demo:v1`
9. Create the _Deployment_ for the Api
   - `kubectl apply -f ../Infrastructure/stockapi-spec.yaml`
10. Use the Kubernetes Dashboard to determine when the Containers have been created and the pods are ready

##### Deploy an Ingress

11. Execute the following command at the command line
    - `minikube ip`
12. Using the IP listed edit your _/etc/hosts_ file
    - `sudo nano /etc/hosts`
    - Add the following lines
    ```
    <minikube-ip> api.stock.local
    <minikube-ip> app.stock.local
    ```
13. Execute the following _kubectl_ command
    - `kubectl apply -f ../Infrastructure/ingress-spec.yaml`
14. Using _Postman_ or _curl_ you should be able to do the following
    - `GET http://api.stock.local/api/test`
    - Getting a response here will indicate the API is available and deployed

##### Deploy the frontend

15. `cd` into _stock-app_
16. Build the Docker image with the following command
    - `docker build -t <my-repo-name>/app-demo:v1 .`
17. Push the Docker image to Docker Hub
    - `docker push -t <my-repo-name>/app-demo:v1`
18. Create the resources in Kubernetes
    - `kubectl apply -f ../Infrastructure/stockapp-spec.yaml`
19. Use the Kubernetes dashboard to determine when things are deployed
20. Open a web browser (I recommend Firefox as Chrome doesnt always respect _/etc/hosts_) and navigate to `http://app.stock.local`

21. It takes about 5s for the UI to update and show things working
