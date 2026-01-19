import { TestAuth } from "@/lib/actions/auth-actions";
import { handleError, successToast } from "@/lib/util";
import { Button } from "@heroui/react";

export default function AuthTestButton() {

    const onClick = async () => {

        const {data, error} = await TestAuth();

        if(error) {
            handleError(error);
        }

        if(data) successToast(data);
    }


  return (
    <Button color='success' 
    onPress={onClick}>
        Test Auth
    </Button>
  )
}
