import { Answer } from "@/lib/types";
import { formatDateLong } from "@/lib/utils/date";
import { Avatar, Chip } from "@heroui/react";
import Link from "next/link";


type Props={
    answer: Answer;    
}

export default function AnswerFooter({answer}:Props) {
  return (
    <div className='flex justify-end mt-4'>
        <div className='flex flex-col basis-2/5 bg-primary/10 dark:bg-primary/20 px-4 py-3 gap-2 rounded-lg border border-primary/20'>
            <span className='text-xs text-foreground-500'>answered {formatDateLong(answer.createdAt)}</span>
            <div className='flex items-center gap-3'>
                <Avatar className='h-7 w-7' color="secondary" name={answer.userDisplayName.charAt(0).toUpperCase()}/>
                <div className='flex flex-col'>
                    <Link href={`/profiles/${answer.userId}`} className='text-sm font-medium text-secondary hover:underline'>
                        {answer.userDisplayName}
                    </Link>
                    <span className='text-xs text-foreground-400 font-semibold'>42</span>
                </div>
            </div>
        </div>
    </div>
  )
}
