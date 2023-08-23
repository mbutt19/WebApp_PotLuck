
echo ==== Logging to AWS ===
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 468850372892.dkr.ecr.us-east-1.amazonaws.com
echo ==== AWS Logging is done ===
echo
echo ==== Docker Buiild ===
echo docker build -t potluck-web .
echo ==== Docker build is done ===
echo
echo ==== Tagging the Docker Image ===
docker tag potluck-web:latest 468850372892.dkr.ecr.us-east-1.amazonaws.com/potluck-web:latest
echo ==== Docker Image is tagged ===
echo
echo ==== Pushing the Docker Image to AWS ===
docker push 468850372892.dkr.ecr.us-east-1.amazonaws.com/potluck-web:latest
echo ==== Docker Image is pushed to AWS ===
echo

