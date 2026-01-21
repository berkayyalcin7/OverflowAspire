'use client';
import { Button } from "@heroui/react";

export default function RegisterButton() {
    // Server Component: SSR'da process.env kullanılabilir, NEXT_PUBLIC gerekmez.
    const clientId = 'nextjs';
    const issuer = process.env.AUTH_KEYCLOAK_ISSUER ?? 'http://localhost:6001/realms/overflow';
    const redirectUrl = process.env.AUTH_URL ?? 'http://localhost:3000';

    // Eksik env varsa butonu render etme, hydration mismatch yaşamayız.
    if (!issuer || !redirectUrl) {
        return null;
    }

    const registerUrl =
        `${issuer}/protocol/openid-connect/registrations` +
        `?client_id=${clientId}&redirect_uri=` +
        `${encodeURIComponent(redirectUrl as string)}&response_type=code&scope=openid`;

    return (
        <Button color='secondary' as='a' href={registerUrl} target="_blank">
            Register
        </Button>
    );
}
