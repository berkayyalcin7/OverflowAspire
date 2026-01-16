'use client';
import { Question } from "@/lib/types";
import { formatDateLong } from "@/lib/utils/date";
import { Avatar, Chip } from "@heroui/react";
import Link from "next/link";

type Props={
    question: Question;
}

export default function QuestionFooter({question}:Props) {
  return (
    <div className='flex justify-between items-end mt-4'>
        <div className='flex flex-wrap gap-2'>
            {question.tagSlugs.map(tag=>(
                <Chip 
                    as={Link} 
                    variant="bordered" 
                    key={tag} 
                    href={`/questions?tag=${tag}`} 
                    className='text-xs hover:bg-primary/10 transition-colors'
                >
                    {tag}
                </Chip>
            ))}
        </div>

        <div className='flex flex-col basis-2/5 bg-primary/10 dark:bg-primary/20 px-4 py-3 gap-2 rounded-lg border border-primary/20'>
            <span className='text-xs text-foreground-500'>asked {formatDateLong(question.createdAt)}</span>
            <div className='flex items-center gap-3'>
                <Avatar className='h-7 w-7' color="secondary" name={question.askerDisplayName.charAt(0).toUpperCase()}/>
                <div className='flex flex-col'>
                    <Link href={`/profiles/${question.askerId}`} className='text-sm font-medium text-secondary hover:underline'>
                        {question.askerDisplayName}
                    </Link>
                    <span className='text-xs text-foreground-400 font-semibold'>42</span>
                </div>
            </div>
        </div>
    </div>
  )
}
