import grpc
from grpc_reflection.v1alpha.proto_reflection_descriptor_database import ProtoReflectionDescriptorDatabase
from google.protobuf.descriptor_pool import DescriptorPool
from grpc_requests import Client

PORT = "5264"
ADDRESS = f"localhost:{PORT}"

def request_using_reflection():
    channel = grpc.insecure_channel(ADDRESS)
    reflection_db = ProtoReflectionDescriptorDatabase(channel)
    pool = DescriptorPool(reflection_db)
    
    try:
        dynamic_client = Client.get_by_endpoint(ADDRESS)
        service_names = reflection_db.get_services()
    except Exception as e:
        print(f"ERROR WHILE GETTING SERVICES: {e}")
        return

    available_methods = []

    for service_name in service_names:

        if service_name == "grpc.reflection.v1alpha.ServerReflection":
            continue
        
        try:
            service_descriptor = pool.FindServiceByName(service_name)
            for service_method in service_descriptor.methods:
                available_methods.append({
                    "service": service_name,
                    "method_name": service_method.name,
                    "input_type": service_method.input_type.name,
                    "output_type": service_method.output_type.name
                })
        except KeyError:
            continue
    
    if not available_methods:
        print("NO METHODS FOUND ON SERVER")
        return
    
    while True:
        for i, method in enumerate(available_methods):
            print(f"{i+1}. {method["method_name"]} ({method["service"]})")
        print("0. Exit")

        choice = input("Choose method to execute: ")

        if choice == "0":
            break

        try:
            choice_index = int(choice)

            if 1 <= choice_index <= len(available_methods):
                selected_method = available_methods[choice_index-1]

                response = dynamic_client.request(selected_method['service'], selected_method['method_name'], {})

                if isinstance(response, dict):
                        print(response)
                else:
                    for msg in response:
                        print(msg)
            else:
                print(f"THERE IS NO METHOD WITH INDEX {choice_index}")
        except ValueError:
            print("INPUT PROPER VALUE")

if __name__ == '__main__':
    request_using_reflection()