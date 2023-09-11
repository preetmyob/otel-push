function Get-HelloWorld {
    Write-Output "Hello, World!"
}

function Get-Square {
    param(
        [int]$Number
    )

    $Square = $Number * $Number
    Write-Output "The square of $Number is $Square"
}
